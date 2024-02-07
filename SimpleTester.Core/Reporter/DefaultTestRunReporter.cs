using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using SimpleTester.Core.Entities;

namespace SimpleTester.Core.Reporter;

public sealed class DefaultTestRunReporter<TOutput>(IList innerTestResult, MethodInfo methodInfo)
    : BaseTestRunReporter<TOutput>(innerTestResult, methodInfo)
{
    public override void Report()
    {
        var logger = BuildLoggerFactory().CreateLogger<DefaultTestRunReporter<TOutput>>();

        var formattedParameters = string.Join(", ", MethodInfo.GetParameters().Select(x => x.ParameterType.Name));
        logger.LogInformation("Executing tests for target method [{returnType} {className}.{methodName}({parameters})]",
            MethodInfo.ReturnType.Name, MethodInfo.DeclaringType, MethodInfo.Name, formattedParameters);

        if (InnerTestResults.Count == 0)
        {
            logger.LogError("No tests added");
            return;
        }

        logger.LogInformation("Total tests: {count}", InnerTestResults.Count);

        var testsToExecute = InnerTestResults
            .Where(x => x.ShouldBeCalculated)
            .ToList();

        if (testsToExecute.Count == 0)
        {
            logger.LogError("No tests to execute");
            return;
        }

        logger.LogInformation("Tests to execute: {count}", testsToExecute.Count);

        var failedTestResults = testsToExecute
            .Where(x => !x.LazyResult.Value.Passed)
            .ToList();

        foreach (var testResult in failedTestResults)
            PrintResult(logger, testResult);

        var totalElapsedMs = testsToExecute.Sum(x => x.LazyResult.Value.ElapsedTime.TotalMilliseconds);
        logger.LogInformation(
            $"Elapsed total: {totalElapsedMs:F5}ms; Avg: {totalElapsedMs / testsToExecute.Count:F5}ms");

        if (failedTestResults.Count == 0)
        {
            logger.LogInformation($"All {testsToExecute.Count} tests passed!");
            return;
        }

        logger.LogError($"{failedTestResults.Count}/{testsToExecute.Count} tests haven't passed!\n Failed tests: [{string.Join(", ", failedTestResults.Select(x => x.Iteration))}]");
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