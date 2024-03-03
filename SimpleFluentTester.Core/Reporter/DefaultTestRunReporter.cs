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

internal sealed class DefaultTestRunReporter<TOutput>(TestRunResult<TOutput> testRunResult)
    : BaseTestRunReporter<TOutput>(testRunResult)
{
    private readonly TestRunResult<TOutput> _testRunResult = testRunResult;

    protected override void ReportInternal()
    {
        if (_testRunResult.Ignored)
            return;
        
        var logger = BuildLoggerFactory().CreateLogger<DefaultTestRunReporter<TOutput>>();
      

        if (!CheckNoTests(logger, _testRunResult.ValidatedTestCases))
            return;
        
        var executedTestCases = _testRunResult.ValidatedTestCases
            .Where(x => x.AssertStatus != AssertStatus.Unknown)
            .ToList();
        
        var stringBuilder = new StringBuilder();

        AppendHeader(stringBuilder,
            _testRunResult.ValidatedTestCases,
            executedTestCases,
            _testRunResult.OperationMethodInfo);

        var printableTestCases = _testRunResult.ValidatedTestCases
            .Where(x => x.AssertStatus == AssertStatus.NotPassed || x.ValidationStatus != ValidationStatus.Valid)
            .ToList();

        foreach (var printableTestCase in printableTestCases)
            AppendResult(stringBuilder, printableTestCase);

        AppendFooter(stringBuilder, executedTestCases, printableTestCases);

        var someTestCasesNotPassed = _testRunResult.ValidatedTestCases
            .Any(x => x.AssertStatus is not (AssertStatus.Passed or AssertStatus.Unknown));
        var someTestCasesNotValid = _testRunResult.ValidatedTestCases
            .Any(x => x.ValidationStatus != ValidationStatus.Valid);
        var suiteContextNotValid = _testRunResult.ContextValidationResults
            .Any(x => !x.IsValid);

        var printedMessage = stringBuilder.ToString();

        if (someTestCasesNotPassed || someTestCasesNotValid || suiteContextNotValid)
            logger.LogError(printedMessage);
        else
            logger.LogInformation(printedMessage);
    }

    private static bool CheckNoTests(ILogger logger, IList<ValidatedTestCase<TOutput>>? testCases)
    {
        if (testCases != null && testCases.Count != 0) 
            return true;
        
        logger.LogError("No test cases were added.\n");
        return false;
    }

    private void AppendHeader(StringBuilder stringBuilder,
        IList<ValidatedTestCase<TOutput>> testCases, 
        IList<ValidatedTestCase<TOutput>> testCasesToExecute, 
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
        ICollection<ValidatedTestCase<TOutput>> executedTestCases, 
        ICollection<ValidatedTestCase<TOutput>> printableTestCases)
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
                .Select(x => x.TestCase.Number)
                .ToList();
            if (failedTestCaseNumbers.Any())
            {
                stringBuilder.Append("Failed test cases: ");
                stringBuilder.AppendLine(string.Join(", ", failedTestCaseNumbers));
            }
            
            var nonValidTestCaseNumbers =  executedTestCases
                .Where(x => x.ValidationStatus == ValidationStatus.NonValid)
                .Select(x => x.TestCase.Number)
                .ToList();
            if (nonValidTestCaseNumbers.Any())
            {
                stringBuilder.Append("Non-valid test cases: ");
                stringBuilder.AppendLine(string.Join(", ", nonValidTestCaseNumbers));
            }
        }

        AppendStatisticsString(stringBuilder, executedTestCases);
    }

    private static void AppendStatisticsString(StringBuilder stringBuilder, ICollection<ValidatedTestCase<TOutput>> executedTestCases)
    {
        if (!executedTestCases.Any())
            return;
        var totalElapsedMs = executedTestCases.Sum(x => x.TestCase.Assert.Value.ElapsedTime.TotalMilliseconds);
        var avgElapsedMs = totalElapsedMs / executedTestCases.Count;
        var maxElapsedTest = executedTestCases.OrderByDescending(x => x.TestCase.Assert.Value.ElapsedTime).First();
        var statistics = $"Elapsed total: {totalElapsedMs:F5}ms; Avg: {avgElapsedMs:F5}ms; Max: {maxElapsedTest.TestCase.Assert.Value.ElapsedTime.TotalMilliseconds:F5}ms [Number {maxElapsedTest.TestCase.Number}]";
        stringBuilder.AppendLine(statistics);
    }
    
    private static void AppendResult(StringBuilder stringBuilder, ValidatedTestCase<TOutput> validatedTestCase)
    {
        var formattedTestCase = FormatTestCaseToString(validatedTestCase);
        stringBuilder.AppendLine(formattedTestCase);
    }
    
    private static string FormatTestCaseToString(ValidatedTestCase<TOutput> validatedTestCase)
    {
        var stringBuilder = new StringBuilder();

        var testCase = validatedTestCase.TestCase;
        
        if (validatedTestCase.ValidationStatus == ValidationStatus.NonValid)
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with a validation error:");
            foreach (var validationResult in validatedTestCase.ValidationResults.Where(x => !x.IsValid))
                AppendValidationResult(stringBuilder, validationResult);
        }

        switch (validatedTestCase.AssertStatus)
        {
            case AssertStatus.Unknown:
                stringBuilder.AppendLine($"Test case [{testCase.Number}] not calculated");
                AddInputString(testCase, stringBuilder);
                AddExpectedString(testCase, stringBuilder);
                return stringBuilder.ToString();
            case AssertStatus.NotPassedWithException:
            {
                stringBuilder.AppendLine($"Test case [{testCase.Number}] not passed with an exception");
                var exception = testCase.Assert.Value.Exception;
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

        if (validatedTestCase.ValidationStatus == ValidationStatus.Valid)
        {
            var assert = validatedTestCase.TestCase.Assert.Value;
            if (assert.Output != null && validatedTestCase.AssertStatus != AssertStatus.NotPassedWithException)
                stringBuilder.AppendLine($"\tOutput: '{assert.Output.Value}'");
            stringBuilder.Append($"\tElapsed: {assert.ElapsedTime.TotalMilliseconds:F5}ms");
        }

        return stringBuilder.ToString();
    }

    private static StringBuilder AddInputString(TestCase<TOutput> testCase, StringBuilder stringBuilder)
    {
        if (testCase.Inputs.Length == 1)
            stringBuilder.AppendLine($"\tInput: '{testCase.Inputs}'");
        else
            stringBuilder.AppendLine($"\tInputs: {string.Join(", ", testCase.Inputs.Select(x => $"'{x}'"))}");
        return stringBuilder;
    }
    
    private static StringBuilder AddExpectedString(TestCase<TOutput> testCase, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"\tExpected: '{testCase.Expected}'");
        return stringBuilder;
    }
}
