using System.Text;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Examples;

internal sealed class CustomTestSuiteReportBuilder : ITestSuiteReportBuilder
{
    public PrintableTestSuiteResult? TestSuiteResultToString(TestSuiteRunResult testSuiteRunResult, Func<AssertedTestClause, AssertedTestCase, bool>? shouldPrintPredicate)
    {
        if (!testSuiteRunResult.ShouldBeExecuted)
            return null;
        
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Custom console test reporter example");
        stringBuilder.AppendLine($"Executing tests for target method [{testSuiteRunResult.Operation?.Method}]");
        stringBuilder.AppendLine($"Total tests: {testSuiteRunResult.TestCases.Count}");
        stringBuilder.AppendLine($"Passed tests: {CountPassedTests(testSuiteRunResult.TestCases)}");
        return new PrintableTestSuiteResult(LogLevel.Information, testSuiteRunResult.Number, stringBuilder.ToString());
    }

    private int CountPassedTests(IList<ITestCase> testCases)
    {
        return testCases
            .Count(x => x.Clauses
                .All(y =>
                {
                    if (y is not AssertedTestClause assertedTestClause)
                        throw new InvalidOperationException("Assertion clause is not an AssertedTestClause");
                    return assertedTestClause.Assert.Status == AssertStatus.Passed;
                }));
    }
}