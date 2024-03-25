using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Reporter;

public static class TestSuiteReportDefaults
{
    public static LogLevel DetermineLogLevel(this TestSuiteResult testSuiteResult)
    {
        var contextNotValid = !testSuiteResult.Validation.IsValid;
        var someTestCasesNotPassed = testSuiteResult.TestCases
            .Any(x => x.Assert.Status != AssertStatus.Passed);
        var someTestCasesNotValid = testSuiteResult.TestCases
            .Any(x => !x.Validation.IsValid);
        
        if (someTestCasesNotPassed || someTestCasesNotValid || contextNotValid)
            return LogLevel.Error;
        return LogLevel.Information;
    }
    
    public static string ToHeaderString(this TestSuiteResult testSuiteResult)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Executing tests for target method [{testSuiteResult.Operation?.Method}]");
        stringBuilder.AppendLine($"Total tests: {testSuiteResult.TestCases.Count}");
        var executedTestCaseCount = testSuiteResult.TestCases
            .Count(x => x.Assert.Status != AssertStatus.Ignored);
        stringBuilder.AppendLine($"Tests to execute: {executedTestCaseCount}");

        if (!testSuiteResult.Validation.IsValid)
        {
            var validationResults = testSuiteResult.Validation.GetNonValid();
            stringBuilder.AppendLine("Test suite did not pass a validation");
            foreach (var validationResult in validationResults)
                AppendValidationResult(stringBuilder, validationResult);
        }
        return stringBuilder.ToString();
    }
    
    public static string ToFooterString(this TestSuiteResult testSuiteResult)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();

        var testCasesGroupedByAssert = testSuiteResult.TestCases
            .ToLookup(x => x.Assert.Status);
        var executedTestCasesGroupedByAssert = testCasesGroupedByAssert
            .Where(x => x.Key is not AssertStatus.Ignored)
            .ToList();
        var executedTestCases = executedTestCasesGroupedByAssert
            .SelectMany(x => x)
            .ToList();
        
        var passedTestCaseNumbers = executedTestCasesGroupedByAssert
            .Where(x => x.Key is AssertStatus.Passed)
            .SelectMany(x => x.Select(y => y.Number))
            .ToList();
        if (passedTestCaseNumbers.Count == testSuiteResult.TestCases.Count)
        {
            stringBuilder.AppendLine($"{passedTestCaseNumbers.Count}/{executedTestCases.Count} tests passed!");
            AppendStatisticsString(stringBuilder, executedTestCases);
            return stringBuilder.ToString();
        }

        var notValidTestCaseNumbers = testCasesGroupedByAssert
            .Where(x => x.Key is AssertStatus.Ignored)
            .SelectMany(x => x)
            .Where(x => !x.Validation.IsValid)
            .Select(x => x.Number)
            .ToList();
        var notPassedTestCaseNumbers = executedTestCasesGroupedByAssert
            .Where(x => x.Key is AssertStatus.NotPassed)
            .SelectMany(x => x.Select(y => y.Number))
            .ToList();
        var notPassedWithExceptionTestCaseNumbers = executedTestCasesGroupedByAssert
            .Where(x => x.Key is AssertStatus.NotPassedWithException)
            .SelectMany(x => x.Select(y => y.Number))
            .ToList();
        var failedTestCaseNumbers = executedTestCasesGroupedByAssert
            .Where(x => x.Key is AssertStatus.Failed)
            .SelectMany(x => x.Select(y => y.Number))
            .ToList();
            
        var totalFailedTestCasesCount = notValidTestCaseNumbers.Count +
                                        notPassedTestCaseNumbers.Count +
                                        notPassedWithExceptionTestCaseNumbers.Count +
                                        failedTestCaseNumbers.Count;
        stringBuilder.Append(executedTestCases.Count - totalFailedTestCasesCount + notValidTestCaseNumbers.Count);
        stringBuilder.Append('/');
        stringBuilder.Append(executedTestCases.Count + notValidTestCaseNumbers.Count);
        stringBuilder.AppendLine($" test cases have been passed, {totalFailedTestCasesCount} test case failed");
            
        if (notPassedTestCaseNumbers.Any())
        {
            stringBuilder.Append("Not passed test cases numbers: ");
            stringBuilder.AppendLine(string.Join(", ", notPassedTestCaseNumbers));
        }
            
        if (notPassedWithExceptionTestCaseNumbers.Any())
        {
            stringBuilder.Append("Failed test cases with exceptions numbers: ");
            stringBuilder.AppendLine(string.Join(", ", notPassedWithExceptionTestCaseNumbers));
        }
            
        if (notValidTestCaseNumbers.Any())
        {
            stringBuilder.Append("Non-valid test cases numbers: ");
            stringBuilder.AppendLine(string.Join(", ", notValidTestCaseNumbers));
        }
            
        if (failedTestCaseNumbers.Any())
        {
            stringBuilder.Append("Test cases numbers with failed assertion: ");
            stringBuilder.AppendLine(string.Join(", ", failedTestCaseNumbers));
        }

        AppendStatisticsString(stringBuilder, executedTestCases);

        return stringBuilder.ToString();
    }
    
    public static string ToFormattedString(this CompletedTestCase testCase)
    {
        var stringBuilder = new StringBuilder();
        
        if (!testCase.Validation.IsValid)
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with a validation error:");
            var validationResults = testCase.Validation.GetNonValid();
            foreach (var validationResult in validationResults)
                AppendValidationResult(stringBuilder, validationResult);
        }

        if (testCase.Assert.Status == AssertStatus.Ignored)
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not calculated");
            AppendInput(testCase, stringBuilder);
            AppendExpected(testCase, stringBuilder);
            return stringBuilder.ToString();
        }

        switch (testCase.Assert.Status)
        {
            case AssertStatus.NotPassedWithException:
                stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with an exception");
                if (testCase.Assert.Output.Value is not Exception exception)
                    break;
                stringBuilder.AppendLine($"Exception: {exception.Message}");
                break;
            case AssertStatus.Passed:
                stringBuilder.AppendLine($"Test case [{testCase.Number}] passed");
                break;
            case AssertStatus.NotPassed:
                stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed");
                break;
            case AssertStatus.Ignored:
                break;
            case AssertStatus.Failed:
                stringBuilder.AppendLine($"Test case [{testCase.Number}] assertion has failed with an exception");
                if (!string.IsNullOrWhiteSpace(testCase.Assert.Message))
                    stringBuilder.AppendLine($"Exception: {testCase.Assert.Message}");
                stringBuilder.AppendLine($"Exception: {testCase.Assert.Exception}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        AppendInput(testCase, stringBuilder);
        AppendExpected(testCase, stringBuilder);

        if (testCase.Validation.IsValid)
        {
            if (testCase.Assert.Status != AssertStatus.NotPassedWithException)
                stringBuilder.AppendLine($"\tOutput: '{testCase.Assert.Output}'");
            stringBuilder.Append($"\tElapsed: {testCase.ElapsedTime.TotalMilliseconds:F5}ms");
        }

        return stringBuilder.ToString();
    }
    
    private static void AppendValidationResult(StringBuilder stringBuilder, ValidationResult validationResult)
    {
        stringBuilder.AppendLine("\t-Validation subject: " + validationResult.Subject);
        stringBuilder.AppendLine("\tError message: " + validationResult.Message);
    }

    private static void AppendStatisticsString(StringBuilder stringBuilder, ICollection<CompletedTestCase> executedTestCases)
    {
        if (executedTestCases.Count == 0)
            return;
        
        var totalElapsedMs = executedTestCases.Sum(x => x.ElapsedTime.TotalMilliseconds);
        var avgElapsedMs = totalElapsedMs / executedTestCases.Count;
        var maxElapsedTest = executedTestCases.OrderByDescending(x => x.ElapsedTime).First();
        var statistics = $"Elapsed total: {totalElapsedMs:F5}ms; Avg: {avgElapsedMs:F5}ms; Max: {maxElapsedTest.ElapsedTime.TotalMilliseconds:F5}ms [Number {maxElapsedTest.Number}]";
        stringBuilder.Append(statistics);
    }

    private static void AppendInput(CompletedTestCase testCase, StringBuilder stringBuilder)
    {
        if (testCase.Inputs.Length == 1)
            stringBuilder.AppendLine($"\tInput: '{testCase.Inputs}'");
        else
            stringBuilder.AppendLine($"\tInputs: {string.Join(", ", testCase.Inputs.Select(x => $"'{x}'"))}");
    }
    
    private static void AppendExpected(CompletedTestCase testCase, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"\tExpected: '{testCase.Expected}'");
    }
}