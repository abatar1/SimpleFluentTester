using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Examples;

internal sealed class CustomTestSuiteReportBuilder<TOutput>(TestSuiteResult<TOutput> testRunResult) : ITestSuiteReportBuilder<TOutput>
{
    public PrintableTestSuiteResult TestSuiteResultToString(TestSuiteResult<TOutput> testSuiteResult, 
        Func<CompletedTestCase<TOutput>, bool> shouldPrintPredicate)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Custom console test reporter example");
        stringBuilder.AppendLine($"Executing tests for target method [{testRunResult.Operation?.Method}]");
        stringBuilder.AppendLine($"Total tests: {testRunResult.TestCases.Count}");
        stringBuilder.AppendLine($"Passed tests: {testRunResult.TestCases.Count(x => x.Assert?.Status == AssertStatus.Passed)}");
        return new PrintableTestSuiteResult(LogLevel.Information, testSuiteResult.Number, stringBuilder.ToString());
    }
}