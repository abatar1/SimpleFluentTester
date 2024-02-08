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
        var avgElapsedMs = totalElapsedMs / testsToExecute.Count;
        var maxElapsedTest = testsToExecute.OrderByDescending(x => x.LazyResult.Value.ElapsedTime).First();
        logger.LogInformation($"Elapsed total: {totalElapsedMs:F5}ms; Avg: {avgElapsedMs:F5}ms; Max: {maxElapsedTest.LazyResult.Value.ElapsedTime.TotalMilliseconds:F5}ms [Iteration {maxElapsedTest.Iteration}]");

        if (failedTestResults.Count == 0)
        {
            logger.LogInformation($"All {testsToExecute.Count} tests passed!");
            return;
        }
        
        var failedMessageBuilder = new StringBuilder();
        failedMessageBuilder.Append(failedTestResults.Count);
        failedMessageBuilder.Append('/');
        failedMessageBuilder.Append(testsToExecute.Count);
        failedMessageBuilder.AppendLine(" tests haven't passed!");
        failedMessageBuilder.Append("Failed test iterations: ");
        failedMessageBuilder.Append(string.Join(", ", failedTestResults.Select(x => x.Iteration)));
        logger.LogError(failedMessageBuilder.ToString());
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