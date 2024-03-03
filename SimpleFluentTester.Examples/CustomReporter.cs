using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Examples;

internal sealed class CustomReporter<TOutput>(TestRunResult<TOutput> testRunResult) : BaseTestRunReporter<TOutput>(testRunResult)
{
    protected override void ReportInternal()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Custom console test reporter example");
        stringBuilder.AppendLine($"Executing tests for target method [{testRunResult.OperationMethodInfo}]");
        stringBuilder.AppendLine($"Total tests: {testRunResult.ValidatedTestCases.Count}");
        var logger = BuildLoggerFactory().CreateLogger<CustomReporter<TOutput>>();
        logger.LogInformation(stringBuilder.ToString());
    }
}