using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestSuiteReportBuilder<TOutput> : ITestSuiteReportBuilder<TOutput>
{
    public PrintableTestSuiteResult? TestSuiteResultToString(TestSuiteResult<TOutput> testSuiteResult)
    {
        if (testSuiteResult.Ignored)
            return null;

        if (!testSuiteResult.TestCases.Any())
            return new PrintableTestSuiteResult(LogLevel.Error, testSuiteResult.Number, "No test cases were added");
        
        var executedTestCases = testSuiteResult.TestCases
            .Where(x => x.AssertStatus != AssertStatus.Unknown)
            .ToList();
        
        var nonValidContextResults = testSuiteResult.ValidationResults
            .SelectMany(x => x.Value)
            .Where(x => !x.IsValid)
            .ToList();
        
        var stringBuilder = new StringBuilder();

        AppendHeader(stringBuilder, testSuiteResult, nonValidContextResults, executedTestCases);

        var printableTestCases = testSuiteResult.TestCases
            .Where(x => x.AssertStatus == AssertStatus.NotPassed || x.ValidationStatus != ValidationStatus.Valid)
            .ToList();

        foreach (var printableTestCase in printableTestCases)
            AppendResult(stringBuilder, printableTestCase);

        AppendFooter(stringBuilder, executedTestCases, printableTestCases);

        var someTestCasesNotPassed = testSuiteResult.TestCases
            .Any(x => x.AssertStatus is not (AssertStatus.Passed or AssertStatus.Unknown));
        var someTestCasesNotValid = testSuiteResult.TestCases
            .Any(x => x.ValidationStatus != ValidationStatus.Valid);
        
        LogLevel logLevel;
        if (someTestCasesNotPassed || someTestCasesNotValid || nonValidContextResults.Count != 0)
            logLevel = LogLevel.Error;
        else
            logLevel = LogLevel.Information;
        return new PrintableTestSuiteResult(logLevel, testSuiteResult.Number, stringBuilder.ToString());
    }

    private static void AppendHeader(StringBuilder stringBuilder,
        TestSuiteResult<TOutput> testRunResult,
        IList<ValidationResult> validationResults,
        IList<CompletedTestCase<TOutput>> testCasesToExecute)
    {
        stringBuilder.AppendLine($"Executing tests for target method [{testRunResult.Operation?.Method}]");
        stringBuilder.AppendLine($"Total tests: {testRunResult.TestCases.Count}");
        stringBuilder.AppendLine($"Tests to execute: {testCasesToExecute.Count}");
        
        if (validationResults.Count != 0)
        {
            stringBuilder.AppendLine("Test suite did not pass a validation");
            foreach (var validationResult in validationResults)
                AppendValidationResult(stringBuilder, validationResult);
        }
        stringBuilder.AppendLine();
    }

    private static void AppendValidationResult(StringBuilder stringBuilder, ValidationResult validationResult)
    {
        stringBuilder.AppendLine("\t-Validation subject: " + validationResult.Subject);
        stringBuilder.AppendLine("\tError message: " + validationResult.Message);
    }

    private static void AppendFooter(StringBuilder stringBuilder, 
        ICollection<CompletedTestCase<TOutput>> executedTestCases, 
        ICollection<CompletedTestCase<TOutput>> printableTestCases)
    {
        stringBuilder.AppendLine();
        
        if (printableTestCases.Count == 0)
        {
            stringBuilder.AppendLine($"All {executedTestCases.Count} tests passed!");
        }
        else
        {
            stringBuilder.Append(printableTestCases.Count);
            stringBuilder.Append('/');
            stringBuilder.Append(executedTestCases.Count);
            stringBuilder.AppendLine(" tests haven't passed!");

            var failedTestCaseNumbers = executedTestCases
                .Where(x => x.AssertStatus == AssertStatus.NotPassed)
                .Select(x => x.Number)
                .ToList();
            if (failedTestCaseNumbers.Any())
            {
                stringBuilder.Append("Failed test cases: ");
                stringBuilder.AppendLine(string.Join(", ", failedTestCaseNumbers));
            }
            
            var nonValidTestCaseNumbers =  executedTestCases
                .Where(x => x.ValidationStatus == ValidationStatus.NonValid)
                .Select(x => x.Number)
                .ToList();
            if (nonValidTestCaseNumbers.Any())
            {
                stringBuilder.Append("Non-valid test cases: ");
                stringBuilder.AppendLine(string.Join(", ", nonValidTestCaseNumbers));
            }
        }

        AppendStatisticsString(stringBuilder, executedTestCases);
    }

    private static void AppendStatisticsString(StringBuilder stringBuilder, ICollection<CompletedTestCase<TOutput>> executedTestCases)
    {
        if (!executedTestCases.Any())
            return;
        var totalElapsedMs = executedTestCases.Sum(x => x.Assert!.ElapsedTime.TotalMilliseconds);
        var avgElapsedMs = totalElapsedMs / executedTestCases.Count;
        var maxElapsedTest = executedTestCases.OrderByDescending(x => x.Assert!.ElapsedTime).First();
        var statistics = $"Elapsed total: {totalElapsedMs:F5}ms; Avg: {avgElapsedMs:F5}ms; Max: {maxElapsedTest.Assert!.ElapsedTime.TotalMilliseconds:F5}ms [Number {maxElapsedTest.Number}]";
        stringBuilder.AppendLine(statistics);
    }
    
    private static void AppendResult(StringBuilder stringBuilder, CompletedTestCase<TOutput> validatedTestCase)
    {
        var formattedTestCase = FormatTestCaseToString(validatedTestCase);
        stringBuilder.AppendLine(formattedTestCase);
    }
    
    private static string FormatTestCaseToString(CompletedTestCase<TOutput> testCase)
    {
        var stringBuilder = new StringBuilder();
        
        if (testCase.ValidationStatus == ValidationStatus.NonValid)
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with a validation error:");
            foreach (var validationResult in testCase.ValidationResults.Where(x => !x.IsValid))
                AppendValidationResult(stringBuilder, validationResult);
        }

        switch (testCase.AssertStatus)
        {
            case AssertStatus.Unknown:
                stringBuilder.AppendLine($"Test case [{testCase.Number}] not calculated");
                AddInputString(testCase, stringBuilder);
                AddExpectedString(testCase, stringBuilder);
                return stringBuilder.ToString();
            case AssertStatus.NotPassedWithException:
            {
                stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with an exception");
                var exception = testCase.Assert!.Exception;
                if (exception is TargetInvocationException targetInvocationException)
                    exception = targetInvocationException.InnerException;
                stringBuilder.AppendLine();
                stringBuilder.Append($"Exception: {exception}");
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

        AddInputString(testCase, stringBuilder);
        AddExpectedString(testCase, stringBuilder);

        if (testCase.ValidationStatus == ValidationStatus.Valid)
        {
            var assert = testCase.Assert;
            if (assert!.Output != null && testCase.AssertStatus != AssertStatus.NotPassedWithException)
                stringBuilder.AppendLine($"\tOutput: '{assert.Output.Value}'");
            stringBuilder.Append($"\tElapsed: {assert.ElapsedTime.TotalMilliseconds:F5}ms");
        }

        return stringBuilder.ToString();
    }

    private static StringBuilder AddInputString(CompletedTestCase<TOutput> testCase, StringBuilder stringBuilder)
    {
        if (testCase.Inputs.Length == 1)
            stringBuilder.AppendLine($"\tInput: '{testCase.Inputs}'");
        else
            stringBuilder.AppendLine($"\tInputs: {string.Join(", ", testCase.Inputs.Select(x => $"'{x}'"))}");
        return stringBuilder;
    }
    
    private static StringBuilder AddExpectedString(CompletedTestCase<TOutput> testCase, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"\tExpected: '{testCase.Expected}'");
        return stringBuilder;
    }
}