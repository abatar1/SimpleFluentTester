using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Helpers;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Provides default utility methods for generating reports and determining log levels for test suite run results.
/// </summary>
public static class TestSuiteReportDefaults
{
    /// <summary>
    /// Determines the appropriate logging level based on the provided test suite run result.
    /// </summary>
    /// <param name="testSuiteResult">The result of the test suite run, containing details about the test cases and their statuses.</param>
    /// <returns>The log level indicating the outcome of the test suite run. Returns <see cref="LogLevel.Error"/> if any test case is invalid, not passed, or the test suite itself is invalid. Otherwise, returns <see cref="LogLevel.Information"/>.</returns>
    public static LogLevel DetermineLogLevel(this TestSuiteRunResult testSuiteResult)
    {
        var someTestCasesNotPassed = testSuiteResult.TestCases
            .SelectMany(x => x.Clauses)
            .Cast<AssertedTestClause>()
            .Any(x => x.Assert.Status != AssertStatus.Passed);
        var someTestCasesNotValid = testSuiteResult.TestCases
            .Any(x => !x.IsValid());
        
        if (someTestCasesNotPassed || someTestCasesNotValid || !testSuiteResult.IsValid)
            return LogLevel.Error;
        return LogLevel.Information;
    }

    /// <summary>
    /// Converts the test suite run result into a header string containing an overview of the test suite execution details.
    /// </summary>
    /// <param name="testSuiteResult">The result of the test suite run, containing information about the executed test cases, target operation, and validation status.</param>
    /// <returns>A formatted string representing the header section of the test suite report, including the target method, total number of tests, the number of tests marked for execution, and validation status details.</returns>
    public static string ToHeaderString(this TestSuiteRunResult testSuiteResult)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Executing tests for target method [{testSuiteResult.Operation?.Method}]");
        stringBuilder.Append($"Total tests: {testSuiteResult.TestCases.Count};");
        var executedTestCaseCount = testSuiteResult.TestCases
            .Count(testCase =>
            {
                return testCase.Clauses
                    .Cast<AssertedTestClause>()
                    .All(clause => clause.Assert.Status != AssertStatus.Ignored);
            });
        stringBuilder.AppendLine($" Tests to execute: {executedTestCaseCount}");

        if (!testSuiteResult.IsValid)
        {
            stringBuilder.AppendLine("Test suite did not pass a validation");
            stringBuilder.AppendLine($"The reason: {testSuiteResult.Exception}");
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Converts the test suite run result into a formatted footer string providing summary information
    /// about the execution and validation of the test cases within the suite.
    /// </summary>
    /// <param name="testSuiteResult">The result of the test suite run, containing details about test case executions, their validation statuses, and overall outcomes.</param>
    /// <returns>A string that summarizes the results of the test suite, including passed and failed test case counts and additional statistics.</returns>
    public static string ToFooterString(this TestSuiteRunResult testSuiteResult)
    {
        var stringBuilder = new StringBuilder();
        
        var passedTestCaseNumbers = testSuiteResult.TestCases
            .Where(testCase => testCase.IsValid())
            .Where(testCase => testCase.Clauses.Cast<AssertedTestClause>().All(clause => clause.Assert.Status is AssertStatus.Passed))
            .Select(testCase => testCase.Number)
            .ToList();

        var assertedTestCases = testSuiteResult.TestCases.ToList();
        
        if (passedTestCaseNumbers.Count == testSuiteResult.TestCases.Count)
        {
            stringBuilder.AppendLine($"{passedTestCaseNumbers.Count}/{passedTestCaseNumbers.Count} tests passed!");
            AppendStatisticsString(stringBuilder, assertedTestCases);
            return stringBuilder.ToString();
        }
        
        var notPassedWithExceptionTestCaseNumbers = testSuiteResult.TestCases
            .Where(testCase => testCase.Clauses.Cast<AssertedTestClause>().Any(clause => clause.Assert.Status is AssertStatus.NotPassedWithException))
            .Select(testCase => testCase.Number)
            .ToList();
        
        var failedTestCaseNumbers = testSuiteResult.TestCases
            .Where(testCase => testCase.Clauses.Cast<AssertedTestClause>().Any(clause => clause.Assert.Status is AssertStatus.Failed))
            .Select(testCase => testCase.Number)
            .ToList();
        
        var notPassedTestCaseNumbers = testSuiteResult.TestCases
            .Where(testCase => testCase.Clauses.Cast<AssertedTestClause>().Any(clause => clause.Assert.Status is AssertStatus.NotPassed))
            .Select(testCase => testCase.Number)
            .ToList();
        
        stringBuilder.AppendLine($"Passed: {passedTestCaseNumbers.Count}, Not passed: {notPassedTestCaseNumbers.Count + notPassedWithExceptionTestCaseNumbers.Count}, Failed: {failedTestCaseNumbers.Count}");
        
        if (notPassedTestCaseNumbers.Any())
        {
            stringBuilder.Append("Not passed numbers: ");
            stringBuilder.AppendLine(string.Join(", ", notPassedTestCaseNumbers));
        }
        if (notPassedWithExceptionTestCaseNumbers.Any())
        {
            stringBuilder.Append("Not passed with exception numbers: ");
            stringBuilder.AppendLine(string.Join(", ", notPassedWithExceptionTestCaseNumbers));
        }
        if (failedTestCaseNumbers.Any())
        {
            stringBuilder.Append("Failed numbers: ");
            stringBuilder.AppendLine(string.Join(", ", failedTestCaseNumbers));
        }

        AppendStatisticsString(stringBuilder, assertedTestCases);

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Converts the specified <see cref="AssertedTestCase"/> instance into a formatted string representation,
    /// detailing the test case number, status, inputs, expected output, and any exceptions encountered.
    /// </summary>
    /// <param name="testCase">The asserted test case instance containing details about the test case execution and validation results.</param>
    /// <returns>A string representation of the test case, including its status, inputs, expected output, and any errors or exceptions.
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if an unsupported assertion status is encountered.</returns>
    public static string ToFormattedString(this AssertedTestCase testCase)
    {
        var stringBuilder = new StringBuilder();

        if (!testCase.IsValid())
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with a validation error:");
            var validationResults = testCase.GetNonValidValidations();
            foreach (var validationResult in validationResults)
                AppendValidationResult(1, stringBuilder, validationResult);
        }
        
        var assertedClauses = testCase.Clauses.Cast<AssertedTestClause>().ToList();

        if (assertedClauses.Any(clause => clause.Assert.Status == AssertStatus.Ignored))
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not calculated");
            return stringBuilder.ToString();
        }

        var notSuccessClauses = assertedClauses
            .Where(clause => clause.Assert.Status != AssertStatus.Passed && clause.Assert.Status != AssertStatus.Ignored)
            .ToList();
        if (notSuccessClauses.Count == 0)
        {
            return stringBuilder.ToString();
        }
        
        stringBuilder.AppendLine($"Test case [{testCase.Number}] was not successful");

        if (assertedClauses.Count == 1)
        {
            var failedClause = assertedClauses.First();
            AppendClauseStatus(1, null, failedClause, stringBuilder);
            AppendInput(1, testCase, stringBuilder);
            AppendExpected(1, failedClause, stringBuilder);
            AppendResult(1, failedClause, stringBuilder);
            AppendElapsed(1, testCase, stringBuilder);
            return stringBuilder.ToString();
        }
        
        AppendInput(1, testCase, stringBuilder);
        AppendElapsed(1, testCase, stringBuilder);
        for (var i = 0; i < assertedClauses.Count; i++)
        {
            AppendClauseStatus(1, i, assertedClauses[i], stringBuilder);
            AppendExpected(2, assertedClauses[i], stringBuilder);
            AppendResult(2, assertedClauses[i], stringBuilder);
        }

        return stringBuilder.ToString();
    }

    private static void AppendClauseStatus(int numberOfMargins, int? clauseNumber, AssertedTestClause clause, StringBuilder stringBuilder)
    {
        string prefix;
        if (clauseNumber.HasValue)
            prefix = $"Reason: Clause {clauseNumber.Value}";
        else
            prefix = "Reason: Test case";
        
        switch (clause.Assert.Status)
        {
            case AssertStatus.NotPassedWithException:
                stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}{prefix} not passed with an exception");
                if (clause.Result.Value is not Exception exception)
                    break;
                stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}Exception: {exception.Message}");
                break;
            case AssertStatus.Passed:
                stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}{prefix} passed");
                break;
            case AssertStatus.NotPassed:
                stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}{prefix} not passed");
                break;
            case AssertStatus.Failed:
                stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}{prefix} assertion has failed with an exception");
                if (!string.IsNullOrWhiteSpace(clause.Assert.Message))
                    stringBuilder.AppendLine($"Exception: {clause.Assert.Message}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void AppendStatisticsString(StringBuilder stringBuilder, IList<AssertedTestCase> assertedTestCases)
    {
        if (assertedTestCases.Count == 0)
            return;
        
        var totalElapsedMs = assertedTestCases.Sum(x => x.ElapsedTime.TotalMilliseconds);
        var avgElapsedMs = totalElapsedMs / assertedTestCases.Count;
        var orderedByElapsedTime = assertedTestCases
            .OrderByDescending(x => x.ElapsedTime)
            .ToList();
        var maxElapsedTest = orderedByElapsedTime.First();
        var minElapsedTest = orderedByElapsedTime.Last();

        var statisticsBuilder = new StringBuilder();
        statisticsBuilder.Append($"Elapsed total: {totalElapsedMs:F4}ms;");
        statisticsBuilder.Append($" Avg: {avgElapsedMs:F4}ms;");
        statisticsBuilder.Append($" Max: {maxElapsedTest.ElapsedTime.TotalMilliseconds:F4}ms [Number {maxElapsedTest.Number}];");
        statisticsBuilder.Append($" Min: {minElapsedTest.ElapsedTime.TotalMilliseconds:F4}ms [Number {minElapsedTest.Number}];");
        stringBuilder.Append(statisticsBuilder);
    }
    
    private static void AppendValidationResult(int numberOfMargins, StringBuilder stringBuilder, ValidationResult validationResult)
    {
        stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}Validation subject: " + validationResult.Subject);
        stringBuilder.AppendLine($"{GetMargin(numberOfMargins + 1)}Error message: " + validationResult.Message);
    }

    private static void AppendInput(int numberOfMargins, AssertedTestCase testCase, StringBuilder stringBuilder)
    {
        if (testCase.Inputs.Count == 1)
            stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}Input: '{testCase.Inputs}'");
        else
            stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}Inputs: {string.Join(", ", testCase.Inputs.Select(x => $"'{x}'"))}");
    }

    private static void AppendElapsed(int numberOfMargins, AssertedTestCase testCase, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}Elapsed: {testCase.ElapsedTime.TotalMilliseconds:F5}ms");
    }
    
    private static void AppendExpected(int numberOfMargins, AssertedTestClause clause, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}Expected: '{clause.Expected}'");
    }

    private static void AppendResult(int numberOfMargins, AssertedTestClause clause, StringBuilder stringBuilder)
    {
        if (clause.Assert.Status != AssertStatus.NotPassedWithException)
            stringBuilder.AppendLine($"{GetMargin(numberOfMargins)}Result: '{clause.Result}'");
    }
    
    private static string GetMargin(int numberOfMargins)
    {
        return string.Join("", Enumerable.Repeat("  ", numberOfMargins));
    }
}