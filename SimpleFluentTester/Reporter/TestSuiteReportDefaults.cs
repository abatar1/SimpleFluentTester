using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Reporter;

public static class TestSuiteReportDefaults
{
    public static LogLevel DetermineLogLevel<TOutput>(this TestSuiteResult<TOutput> testSuiteResult)
    {
        var contextNotValid = testSuiteResult.ContextValidation.Status == ValidationStatus.NonValid;
        var someTestCasesNotPassed = testSuiteResult.TestCases
            .Any(x => x.Assert?.Status != AssertStatus.Passed);
        var someTestCasesNotValid = testSuiteResult.TestCases
            .Any(x => x.ValidationStatus != ValidationStatus.Valid);
        
        if (someTestCasesNotPassed || someTestCasesNotValid || contextNotValid)
            return LogLevel.Error;
        return LogLevel.Information;
    }
    
    public static string ToHeaderString<TOutput>(this TestSuiteResult<TOutput> testSuiteResult)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Executing tests for target method [{testSuiteResult.Operation?.Method}]");
        stringBuilder.AppendLine($"Total tests: {testSuiteResult.TestCases.Count}");
        var executedTestCaseCount = testSuiteResult.TestCases
            .Count(x => x.Executed);
        stringBuilder.AppendLine($"Tests to execute: {executedTestCaseCount}");

        if (testSuiteResult.ContextValidation.Status == ValidationStatus.NonValid)
        {
            var validationResults = testSuiteResult.ContextValidation.Results
                .Where(x => !x.IsValid)
                .ToList();
            stringBuilder.AppendLine("Test suite did not pass a validation");
            foreach (var validationResult in validationResults)
                AppendValidationResult(stringBuilder, validationResult);
        }
        stringBuilder.AppendLine();
        return stringBuilder.ToString();
    }
    
    public static string ToFooterString<TOutput>(this TestSuiteResult<TOutput> testSuiteResult)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        
        var executedTestCases = testSuiteResult.TestCases
            .Where(x => x.ValidationStatus != ValidationStatus.Ignored)
            .ToList();
        
        var failedTestCaseNumbers = executedTestCases
            .Where(x => x.Assert?.Status is AssertStatus.NotPassed)
            .Select(x => x.Number)
            .ToList();
        var failedTestCaseWithExceptionsNumbers = executedTestCases
            .Where(x => x.Assert?.Status is AssertStatus.NotPassedWithException)
            .Select(x => x.Number)
            .ToList();
        var nonValidTestCaseNumbers = executedTestCases
            .Where(x => x.ValidationStatus is ValidationStatus.NonValid)
            .Select(x => x.Number)
            .ToList();
        
        if (failedTestCaseNumbers.Count == 0 
            && nonValidTestCaseNumbers.Count == 0 
            && failedTestCaseWithExceptionsNumbers.Count == 0)
        {
            stringBuilder.AppendLine($"All {executedTestCases.Count} tests passed!");
        }
        else
        {
            var totalFailedTestCasesCount = failedTestCaseNumbers.Count +
                                            nonValidTestCaseNumbers.Count +
                                            failedTestCaseWithExceptionsNumbers.Count;
            stringBuilder.Append(totalFailedTestCasesCount);
            stringBuilder.Append('/');
            stringBuilder.Append(executedTestCases.Count);
            stringBuilder.AppendLine(" tests haven't passed!");
            
            if (failedTestCaseNumbers.Any())
            {
                stringBuilder.Append("Failed test cases: ");
                stringBuilder.AppendLine(string.Join(", ", failedTestCaseNumbers));
            }
            
            if (failedTestCaseWithExceptionsNumbers.Any())
            {
                stringBuilder.Append("Failed test cases with exceptions: ");
                stringBuilder.AppendLine(string.Join(", ", failedTestCaseWithExceptionsNumbers));
            }
            
            if (nonValidTestCaseNumbers.Any())
            {
                stringBuilder.Append("Non-valid test cases: ");
                stringBuilder.AppendLine(string.Join(", ", nonValidTestCaseNumbers));
            }
        }

        AppendStatisticsString(stringBuilder, executedTestCases);

        return stringBuilder.ToString();
    }
    
    public static string ToFormattedString<TOutput>(this CompletedTestCase<TOutput> testCase)
    {
        var stringBuilder = new StringBuilder();
        
        if (testCase.ValidationStatus == ValidationStatus.NonValid)
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with a validation error:");
            foreach (var validationResult in testCase.ValidationResults.Where(x => !x.IsValid))
                AppendValidationResult(stringBuilder, validationResult);
        }

        if (!testCase.Executed)
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not calculated");
            AppendInput(testCase, stringBuilder);
            AppendExpected(testCase, stringBuilder);
            return stringBuilder.ToString();
        }

        switch (testCase.Assert?.Status)
        {
            case AssertStatus.NotPassedWithException:
            {
                stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with an exception");
                
                var exception = testCase.Assert?.Exception;
                if (exception == null)
                    break;
                
                if (exception is TargetInvocationException targetInvocationException)
                    exception = targetInvocationException.InnerException;
                stringBuilder.AppendLine($"Exception: {exception?.Message}");
                break;
            }
            case AssertStatus.Passed:
                stringBuilder.AppendLine($"Test case [{testCase.Number}] passed");
                break;
            case AssertStatus.NotPassed:
                stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        AppendInput(testCase, stringBuilder);
        AppendExpected(testCase, stringBuilder);

        if (testCase.ValidationStatus == ValidationStatus.Valid)
        {
            var assert = testCase.Assert;
            if (assert?.Output != null && testCase.Assert?.Status != AssertStatus.NotPassedWithException)
                stringBuilder.AppendLine($"\tOutput: '{assert.Output.Value}'");
            stringBuilder.Append($"\tElapsed: {assert?.ElapsedTime.TotalMilliseconds:F5}ms");
        }

        return stringBuilder.ToString();
    }
    
    private static void AppendValidationResult(StringBuilder stringBuilder, ValidationResult validationResult)
    {
        stringBuilder.AppendLine("\t-Validation subject: " + validationResult.Subject);
        stringBuilder.AppendLine("\tError message: " + validationResult.Message);
    }

    private static void AppendStatisticsString<TOutput>(StringBuilder stringBuilder, ICollection<CompletedTestCase<TOutput>> executedTestCases)
    {
        if (executedTestCases.Count == 0)
            return;
        
        var totalElapsedMs = executedTestCases.Sum(x => x.Assert?.ElapsedTime.TotalMilliseconds);
        var avgElapsedMs = totalElapsedMs / executedTestCases.Count;
        var maxElapsedTest = executedTestCases.OrderByDescending(x => x.Assert?.ElapsedTime).First();
        var statistics = $"Elapsed total: {totalElapsedMs:F5}ms; Avg: {avgElapsedMs:F5}ms; Max: {maxElapsedTest.Assert?.ElapsedTime.TotalMilliseconds:F5}ms [Number {maxElapsedTest.Number}]";
        stringBuilder.AppendLine(statistics);
    }

    private static void AppendInput<TOutput>(CompletedTestCase<TOutput> testCase, StringBuilder stringBuilder)
    {
        if (testCase.Inputs.Length == 1)
            stringBuilder.AppendLine($"\tInput: '{testCase.Inputs}'");
        else
            stringBuilder.AppendLine($"\tInputs: {string.Join(", ", testCase.Inputs.Select(x => $"'{x}'"))}");
    }
    
    private static void AppendExpected<TOutput>(CompletedTestCase<TOutput> testCase, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"\tExpected: '{testCase.Expected}'");
    }
}