using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestSuiteReportBuilder<TOutput> : ITestSuiteReportBuilder<TOutput>
{
    public PrintableTestSuiteResult? TestSuiteResultToString(
        TestSuiteResult<TOutput> testSuiteResult,
        Func<CompletedTestCase<TOutput>, bool> shouldPrintPredicate)
    {
        if (testSuiteResult.ContextValidation.Status == ValidationStatus.Ignored)
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