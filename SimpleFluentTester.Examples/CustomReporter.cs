using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.Suite;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.Examples
{
    internal sealed class CustomReporter<TOutput>(TestSuiteResult<TOutput> testRunResult) : BaseTestRunReporter<TOutput>(testRunResult)
    {
        private readonly TestSuiteResult<TOutput> _testRunResult = testRunResult;

        protected override void ReportInternal()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Custom console test reporter example");
            stringBuilder.AppendLine($"Executing tests for target method [{_testRunResult.OperationMethodInfo}]");
            stringBuilder.AppendLine($"Total tests: {_testRunResult.TestCases.Count}");
            stringBuilder.AppendLine($"Passed tests: {_testRunResult.TestCases.Count(x => x.AssertStatus == AssertStatus.Passed)}");
            var logger = CreateLogger();
            logger.LogInformation(_testRunResult.Number, null, stringBuilder.ToString());
        }
    }
}