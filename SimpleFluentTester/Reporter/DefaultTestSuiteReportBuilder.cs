using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestSuiteReportBuilder : ITestSuiteReportBuilder
{
    public PrintableTestSuiteResult? TestSuiteResultToString(
        TestSuiteRunResult testSuiteRunResult,
        Func<AssertedTestClause, AssertedTestCase, bool>? shouldPrintPredicate)
    {
        if (!testSuiteRunResult.ShouldBeExecuted)
            return null;

        if (!testSuiteRunResult.TestCases.Any())
            return new PrintableTestSuiteResult(LogLevel.Error, testSuiteRunResult.Number, "No test cases were added");
        
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(testSuiteRunResult.ToHeaderString());

        var testCases = testSuiteRunResult.TestCases.Cast<AssertedTestCase>();
        if (shouldPrintPredicate != null)
        {
            testCases = testCases.Where(testCase =>
            {
                return testCase.Clauses
                    .Cast<AssertedTestClause>()
                    .Select(testClause => shouldPrintPredicate.Invoke(testClause, testCase))
                    .Any(x => x);
            });
        }
        var printableTestCases = testCases.ToList();

        foreach (var printableTestCase in printableTestCases)
            stringBuilder.AppendLine(printableTestCase.ToFormattedString());

        stringBuilder.AppendLine(testSuiteRunResult.ToFooterString());

        var logLevel = testSuiteRunResult.DetermineLogLevel();
        return new PrintableTestSuiteResult(logLevel, testSuiteRunResult.Number, stringBuilder.ToString());
    }
}