using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Entities;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestRunReporter<TOutput>(IEnumerable innerTestResult, MethodInfo methodInfo)
    : BaseTestRunReporter<TOutput>(innerTestResult, methodInfo)
{
    public override void Report()
    {
        var logger = BuildLoggerFactory().CreateLogger<DefaultTestRunReporter<TOutput>>();

        var allTestCases = LogHeader(logger, InnerTestResults, MethodInfo);
        if (allTestCases == null)
            return;

        var failedTestCases = allTestCases
            .Where(x => !x.LazyResult.Value.Passed)
            .ToList();

        foreach (var testResult in failedTestCases)
            LogResult(logger, testResult);

        LogFooter(logger, allTestCases, failedTestCases);
    }

    private static IList<TestCase<TOutput>>? LogHeader(ILogger logger, 
        ICollection<TestCase<TOutput>>? testCases, 
        MethodInfo methodInfo)
    {
        var headerStringBuilder = new StringBuilder();
        
        headerStringBuilder.AppendLine($"Executing tests for target method [{methodInfo}]");

        if (testCases.Count == 0)
        {
            headerStringBuilder.AppendLine("The test cases have not been added");
            logger.LogError(headerStringBuilder.ToString());
            return null;
        }

        headerStringBuilder.AppendLine($"Total tests: {testCases.Count}");

        var testsToExecute = testCases
            .Where(x => x.ShouldBeCalculated)
            .ToList();

        if (testsToExecute.Count == 0)
        {
            headerStringBuilder.AppendLine("Not a single test was found to be executed");
            logger.LogError(headerStringBuilder.ToString());
            return null;
        }

        headerStringBuilder.Append($"Tests to execute: {testsToExecute.Count}");
        logger.LogInformation(headerStringBuilder.ToString());

        return testsToExecute;
    }

    private static void LogFooter(ILogger logger, 
        ICollection<TestCase<TOutput>> testCases, 
        ICollection<TestCase<TOutput>> failedTestCases)
    {
        var footerStringBuilder = new StringBuilder();
        LogLevel logLevel;
        
        if (failedTestCases.Count == 0)
        {
            footerStringBuilder.AppendLine($"All {testCases.Count} tests passed!");
            logLevel = LogLevel.Information;
        }
        else
        {
            footerStringBuilder.Append(failedTestCases.Count);
            footerStringBuilder.Append('/');
            footerStringBuilder.Append(testCases.Count);
            footerStringBuilder.AppendLine(" tests haven't passed!");
            footerStringBuilder.Append("Failed test cases: ");
            footerStringBuilder.AppendLine(string.Join(", ", failedTestCases.Select(x => x.Number)));
            logLevel = LogLevel.Error;
        }
        
        var totalElapsedMs = testCases.Sum(x => x.LazyResult.Value.ElapsedTime.TotalMilliseconds);
        var avgElapsedMs = totalElapsedMs / testCases.Count;
        var maxElapsedTest = testCases.OrderByDescending(x => x.LazyResult.Value.ElapsedTime).First();
        var statistics =
            $"Elapsed total: {totalElapsedMs:F5}ms; Avg: {avgElapsedMs:F5}ms; Max: {maxElapsedTest.LazyResult.Value.ElapsedTime.TotalMilliseconds:F5}ms [Number {maxElapsedTest.Number}]";
        footerStringBuilder.Append(statistics);
        
        logger.Log(logLevel, null, footerStringBuilder.ToString());
    }

    private static void LogResult(ILogger logger, TestCase<TOutput> testCase)
    {
        var noError = testCase is { LazyResult.Value: { Passed: true, Exception: null } };
        if (noError)
            logger.LogInformation(testCase.ToString());
        else
            logger.LogError(testCase.ToString());
    }
}
