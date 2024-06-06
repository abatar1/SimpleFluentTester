using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestSuiteReportBuilder : ITestSuiteReportBuilder
{
    public PrintableTestSuiteResult? TestSuiteResultToString(
        TestSuiteRunResult testSuiteRunResult,
        Func<CompletedTestCase, bool>? shouldPrintPredicate)
    {
        if (!testSuiteRunResult.ShouldBeExecuted)
            return null;

        if (!testSuiteRunResult.TestCases.Any())
            return new PrintableTestSuiteResult(LogLevel.Error, testSuiteRunResult.Number, "No test cases were added");
        
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(testSuiteRunResult.ToHeaderString());

        IEnumerable<CompletedTestCase> testCaseEnumerable = testSuiteRunResult.TestCases;
        if (shouldPrintPredicate != null)
            testCaseEnumerable = testCaseEnumerable.Where(shouldPrintPredicate);
        var printableTestCases = testCaseEnumerable.ToList();

        foreach (var printableTestCase in printableTestCases)
            stringBuilder.AppendLine(printableTestCase.ToFormattedString());

        stringBuilder.AppendLine(testSuiteRunResult.ToFooterString());

        var logLevel = testSuiteRunResult.DetermineLogLevel();
        return new PrintableTestSuiteResult(logLevel, testSuiteRunResult.Number, stringBuilder.ToString());
    }
}