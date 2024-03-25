using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Examples;

internal sealed class CustomTestSuiteReportBuilder(TestSuiteResult testRunResult) : ITestSuiteReportBuilder
{
    public PrintableTestSuiteResult? TestSuiteResultToString(TestSuiteResult testSuiteResult, 
        Func<CompletedTestCase, bool> shouldPrintPredicate)
    {
        if (!testSuiteResult.ShouldBeExecuted)
            return null;
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Custom console test reporter example");
        stringBuilder.AppendLine($"Executing tests for target method [{testRunResult.Operation?.Method}]");
        stringBuilder.AppendLine($"Total tests: {testRunResult.TestCases.Count}");
        stringBuilder.AppendLine($"Passed tests: {testRunResult.TestCases.Count(x => x.Assert.Status == AssertStatus.Passed)}");
        return new PrintableTestSuiteResult(LogLevel.Information, testSuiteResult.Number, stringBuilder.ToString());
    }
}