using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Examples;

internal sealed class CustomTestSuiteReportBuilder : ITestSuiteReportBuilder
{
    public PrintableTestSuiteResult? TestSuiteResultToString(TestSuiteRunResult testSuiteRunResult, 
        Func<CompletedTestCase, bool>? shouldPrintPredicate)
    {
        if (!testSuiteRunResult.ShouldBeExecuted)
            return null;
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Custom console test reporter example");
        stringBuilder.AppendLine($"Executing tests for target method [{testSuiteRunResult.Operation?.Method}]");
        stringBuilder.AppendLine($"Total tests: {testSuiteRunResult.TestCases.Count}");
        stringBuilder.AppendLine($"Passed tests: {testSuiteRunResult.TestCases.Count(x => x.Assert.Status == AssertStatus.Passed)}");
        return new PrintableTestSuiteResult(LogLevel.Information, testSuiteRunResult.Number, stringBuilder.ToString());
    }
}