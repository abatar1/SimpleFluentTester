using System;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Reporter;

public interface ITestSuiteReportBuilder<TOutput>
{
    PrintableTestSuiteResult? TestSuiteResultToString(
        TestSuiteResult<TOutput> testSuiteResult,
        Func<CompletedTestCase<TOutput>, bool> shouldPrintPredicate);
}