using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Entities;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestRunReporter<TOutput>(TestSuiteResult<TOutput> testRunResult)
    : BaseTestRunReporter<TOutput>(testRunResult)
{
    private readonly TestSuiteResult<TOutput> _testRunResult = testRunResult;

    protected override void ReportInternal()
    {
        if (_testRunResult.Ignored)
            return;
        
        var logger = CreateLogger();

        if (!CheckNoTests(logger, _testRunResult.TestCases))
            return;
        
        var executedTestCases = _testRunResult.TestCases
            .Where(x => x.AssertStatus != AssertStatus.Unknown)
            .ToList();
        
        var stringBuilder = new StringBuilder();

        AppendHeader(stringBuilder,
            _testRunResult.TestCases,
            executedTestCases,
            _testRunResult.OperationMethodInfo);

        var printableTestCases = _testRunResult.TestCases
            .Where(x => x.AssertStatus == AssertStatus.NotPassed || x.ValidationStatus != ValidationStatus.Valid)
            .ToList();

        foreach (var printableTestCase in printableTestCases)
            AppendResult(stringBuilder, printableTestCase);

        AppendFooter(stringBuilder, executedTestCases, printableTestCases);

        var someTestCasesNotPassed = _testRunResult.TestCases
            .Any(x => x.AssertStatus is not (AssertStatus.Passed or AssertStatus.Unknown));
        var someTestCasesNotValid = _testRunResult.TestCases
            .Any(x => x.ValidationStatus != ValidationStatus.Valid);
        var suiteContextNotValid = _testRunResult.ContextValidationResults
            .Any(x => !x.IsValid);

        var printedMessage = stringBuilder.ToString();

        if (someTestCasesNotPassed || someTestCasesNotValid || suiteContextNotValid)
            logger.LogError(TestRunResult.Number, null, printedMessage);
        else
            logger.LogInformation(TestRunResult.Number, null, printedMessage);
    }

    private static bool CheckNoTests(ILogger logger, IList<CompletedTestCase<TOutput>>? testCases)
    {
        if (testCases != null && testCases.Count != 0) 
            return true;
        
        logger.LogError("No test cases were added.\n");
        return false;
    }

    private void AppendHeader(StringBuilder stringBuilder,
        IList<CompletedTestCase<TOutput>> testCases, 
        IList<CompletedTestCase<TOutput>> testCasesToExecute, 
        MethodInfo? methodInfo)
    {
        stringBuilder.AppendLine($"Executing tests for target method [{methodInfo}]");
        stringBuilder.AppendLine($"Total tests: {testCases.Count}");
        stringBuilder.AppendLine($"Tests to execute: {testCasesToExecute.Count}");

        var nonValidContextResults = _testRunResult.ContextValidationResults
            .Where(x => !x.IsValid)
            .ToList();
        if (nonValidContextResults.Count != 0)
        {
            stringBuilder.AppendLine("Test suite did not pass a validation");
            foreach (var validationResult in nonValidContextResults)
                AppendValidationResult(stringBuilder, validationResult);
        }

        stringBuilder.AppendLine();
    }

    private static void AppendValidationResult(StringBuilder stringBuilder, ValidationResult validationResult)
    {
        stringBuilder.AppendLine("\t-Validation subject: " + validationResult.ValidationSubject);
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
