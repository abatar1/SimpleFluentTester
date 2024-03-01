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

    public override void Report()
    {
        if (testRunResult.Ignored)
            return;
        
        var logger = BuildLoggerFactory().CreateLogger<DefaultTestRunReporter<TOutput>>();

        if (!CheckNoTests(logger, _testRunResult.ValidatedTestCases))
            return;
        
        var executedTestCases = _testRunResult.ValidatedTestCases
            .Where(x => x.AssertStatus != AssertStatus.Unknown)
            .ToList();
        
        if (!CheckNoTestsToExecute(logger, executedTestCases))
            return;

        LogHeader(logger,
            _testRunResult.ValidatedTestCases,
            executedTestCases,
            _testRunResult.OperationMethodInfo);

        var printableTestCases = executedTestCases
            .Where(x => x.AssertStatus == AssertStatus.NotPassed || x.ValidationStatus != ValidationStatus.Valid)
            .ToList();

        foreach (var printableTestCase in printableTestCases)
            LogResult(logger, printableTestCase);

        LogFooter(logger, executedTestCases, printableTestCases);
    }

    private static bool CheckNoTests(ILogger logger, IList<ValidatedTestCase<TOutput>>? testCases)
    {
        if (testCases != null && testCases.Count != 0) 
            return true;
        
        logger.LogError("No test cases were added.\n");
        return false;
    }
    
    private static bool CheckNoTestsToExecute(ILogger logger, IList<ValidatedTestCase<TOutput>>? executedTestCases)
    {
        if (executedTestCases != null && executedTestCases.Count != 0) 
            return true;
        
        logger.LogError("No test cases were executed.\n");
        return false;
    }

    private static void LogHeader(ILogger logger, 
        IList<ValidatedTestCase<TOutput>> testCases, 
        IList<ValidatedTestCase<TOutput>> testCasesToExecute, 
        MethodInfo? methodInfo)
    {
        var headerStringBuilder = new StringBuilder();
        headerStringBuilder.AppendLine($"Executing tests for target method [{methodInfo}]");
        headerStringBuilder.AppendLine($"Total tests: {testCases.Count}");
        headerStringBuilder.Append($"Tests to execute: {testCasesToExecute.Count}");
        logger.LogInformation(headerStringBuilder.ToString());
    }

    private static void LogFooter(ILogger logger, 
        ICollection<ValidatedTestCase<TOutput>> executedTestCases, 
        ICollection<ValidatedTestCase<TOutput>> printableTestCases)
    {
        var footerStringBuilder = new StringBuilder();
        LogLevel logLevel;
        
        if (printableTestCases.Count == 0)
        {
            footerStringBuilder.AppendLine($"All {executedTestCases.Count} tests passed!");
            logLevel = LogLevel.Information;
        }
        else
        {
            footerStringBuilder.Append(printableTestCases.Count);
            footerStringBuilder.Append('/');
            footerStringBuilder.Append(executedTestCases.Count);
            footerStringBuilder.AppendLine(" tests haven't passed!");

            var failedTestCaseNumbers = executedTestCases
                .Where(x => x.AssertStatus == AssertStatus.NotPassed)
                .Select(x => x.TestCase.Number)
                .ToList();
            if (failedTestCaseNumbers.Any())
            {
                footerStringBuilder.Append("Failed test cases: ");
                footerStringBuilder.AppendLine(string.Join(", ", failedTestCaseNumbers));
            }
            
            var nonValidTestCaseNumbers =  executedTestCases
                .Where(x => x.ValidationStatus == ValidationStatus.NonValid)
                .Select(x => x.TestCase.Number)
                .ToList();
            if (nonValidTestCaseNumbers.Any())
            {
                footerStringBuilder.Append("Non-valid test cases: ");
                footerStringBuilder.AppendLine(string.Join(", ", nonValidTestCaseNumbers));
            }
            
            logLevel = LogLevel.Error;
        }
        
        var totalElapsedMs = executedTestCases.Sum(x => x.TestCase.Assert.Value.ElapsedTime.TotalMilliseconds);
        var avgElapsedMs = totalElapsedMs / executedTestCases.Count;
        var maxElapsedTest = executedTestCases.OrderByDescending(x => x.TestCase.Assert.Value.ElapsedTime).First();
        var statistics =
            $"Elapsed total: {totalElapsedMs:F5}ms; Avg: {avgElapsedMs:F5}ms; Max: {maxElapsedTest.TestCase.Assert.Value.ElapsedTime.TotalMilliseconds:F5}ms [Number {maxElapsedTest.TestCase.Number}]";
        footerStringBuilder.Append(statistics);
        
        logger.Log(logLevel, null, footerStringBuilder.ToString());
    }

    private static void LogResult(ILogger logger, ValidatedTestCase<TOutput> validatedTestCase)
    {
        var formattedTestCase = FormatTestCaseToString(validatedTestCase);
        if (validatedTestCase.AssertStatus == AssertStatus.Passed)
            logger.LogInformation(formattedTestCase);
        else
            logger.LogError(formattedTestCase);
    }
    
    private static string FormatTestCaseToString(ValidatedTestCase<TOutput> validatedTestCase)
    {
        var stringBuilder = new StringBuilder();

        var testCase = validatedTestCase.TestCase;

        if (validatedTestCase.AssertStatus == AssertStatus.Unknown)
        {
            stringBuilder.AppendLine($"Test case [{testCase.Number}] not calculated");
            AddInputString(testCase, stringBuilder);
            AddExpectedString(testCase, stringBuilder);
            return stringBuilder.ToString();
        }

        var calculatedAssert = testCase.Assert.Value;

        var noError = calculatedAssert.Passed && calculatedAssert.Exception == null;

        stringBuilder.AppendLine($"Test case [{testCase.Number}] {(!noError ? "not " : "")}passed");

        AddInputString(testCase, stringBuilder);
        AddExpectedString(testCase, stringBuilder);

        if (calculatedAssert.Output != null)
            stringBuilder.AppendLine($"Output: '{calculatedAssert.Output.Value}'");
      
        stringBuilder.Append($"Elapsed: {calculatedAssert.ElapsedTime.TotalMilliseconds:F5}ms");

        if (calculatedAssert.Exception != null)
        {
            var exception = calculatedAssert.Exception;
            if (exception is TargetInvocationException targetInvocationException)
                exception = targetInvocationException.InnerException;
            stringBuilder.AppendLine();
            stringBuilder.Append($"Exception: {exception}");
        }

        return stringBuilder.ToString();
    }

    private static StringBuilder AddInputString(TestCase<TOutput> testCase, StringBuilder stringBuilder)
    {
        if (testCase.Inputs.Length == 1)
            stringBuilder.AppendLine($"Input: '{testCase.Inputs}'");
        else
            stringBuilder.AppendLine($"Inputs: {string.Join(", ", testCase.Inputs.Select(x => $"'{x}'"))}");
        return stringBuilder;
    }
    
    private static StringBuilder AddExpectedString(TestCase<TOutput> testCase, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"Expected: '{testCase.Expected}'");
        return stringBuilder;
    }
}
