using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestSuiteReportBuilder : ITestSuiteReportBuilder
{
    public PrintableTestSuiteResult? TestSuiteResultToString(
        TestSuiteResult testSuiteResult,
        Func<CompletedTestCase, bool> shouldPrintPredicate)
    {
        if (!testSuiteResult.ShouldBeExecuted)
            return null;

        if (!testSuiteResult.TestCases.Any())
            return new PrintableTestSuiteResult(LogLevel.Error, testSuiteResult.Number, "No test cases were added");
        
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(testSuiteResult.ToHeaderString());

        var printableTestCases = testSuiteResult.TestCases
            .Where(shouldPrintPredicate)
            .ToList();

        foreach (var printableTestCase in printableTestCases)
            stringBuilder.AppendLine(printableTestCase.ToFormattedString());

        stringBuilder.AppendLine(testSuiteResult.ToFooterString());

        var logLevel = testSuiteResult.DetermineLogLevel();
        return new PrintableTestSuiteResult(logLevel, testSuiteResult.Number, stringBuilder.ToString());
    }
}