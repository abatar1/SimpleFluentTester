using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Entities;

namespace SimpleFluentTester.Reporter;

public sealed class DefaultTestRunReporter<TOutput>(IList innerTestResult, MethodInfo methodInfo)
    : BaseTestRunReporter<TOutput>(innerTestResult, methodInfo)
{
    public override void Report()
    {
        var logger = BuildLoggerFactory().CreateLogger<DefaultTestRunReporter<TOutput>>();

        var headerStringBuilder = new StringBuilder();
        
        headerStringBuilder.AppendLine($"Executing tests for target method [{MethodInfo}]");

        if (InnerTestResults.Count == 0)
        {
            headerStringBuilder.AppendLine("The test cases have not been added");
            logger.LogError(headerStringBuilder.ToString());
            return;
        }

        headerStringBuilder.AppendLine($"Total tests: {InnerTestResults.Count}");

        var testsToExecute = InnerTestResults
            .Where(x => x.ShouldBeCalculated)
            .ToList();

        if (testsToExecute.Count == 0)
        {
            headerStringBuilder.AppendLine("Not a single test was found to be executed");
            logger.LogError(headerStringBuilder.ToString());
            return;
        }

        headerStringBuilder.Append($"Tests to execute: {testsToExecute.Count}");
        logger.LogInformation(headerStringBuilder.ToString());

        var failedTestResults = testsToExecute
            .Where(x => !x.LazyResult.Value.Passed)
            .ToList();

        foreach (var testResult in failedTestResults)
            PrintResult(logger, testResult);
        
        var outerStringBuilder = new StringBuilder();
        var logLevel = LogLevel.None;
        
        if (failedTestResults.Count == 0)
        {
            outerStringBuilder.AppendLine($"All {testsToExecute.Count} tests passed!");
            logLevel = LogLevel.Information;
        }
        else
        {
            outerStringBuilder.Append(failedTestResults.Count);
            outerStringBuilder.Append('/');
            outerStringBuilder.Append(testsToExecute.Count);
            outerStringBuilder.AppendLine(" tests haven't passed!");
            outerStringBuilder.Append("Failed test cases: ");
            outerStringBuilder.AppendLine(string.Join(", ", failedTestResults.Select(x => x.Number)));
            logLevel = LogLevel.Error;
        }
        
        var totalElapsedMs = testsToExecute.Sum(x => x.LazyResult.Value.ElapsedTime.TotalMilliseconds);
        var avgElapsedMs = totalElapsedMs / testsToExecute.Count;
        var maxElapsedTest = testsToExecute.OrderByDescending(x => x.LazyResult.Value.ElapsedTime).First();
        var statistics =
            $"Elapsed total: {totalElapsedMs:F5}ms; Avg: {avgElapsedMs:F5}ms; Max: {maxElapsedTest.LazyResult.Value.ElapsedTime.TotalMilliseconds:F5}ms [Number {maxElapsedTest.Number}]";
        outerStringBuilder.Append(statistics);
        
        logger.Log(logLevel, null, outerStringBuilder.ToString());
    }

    private static void PrintResult(ILogger logger, TestCase<TOutput> testResult)
    {
        var noError = testResult is { LazyResult.Value: { Passed: true, Exception: null } };
        if (noError)
            logger.LogInformation(testResult.ToString());
        else
            logger.LogError(testResult.ToString());
    }
}
