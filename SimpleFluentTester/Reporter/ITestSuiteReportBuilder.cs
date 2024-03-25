using System;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

public interface ITestSuiteReportBuilder
{
    PrintableTestSuiteResult? TestSuiteResultToString(
        TestSuiteResult testSuiteResult,
        Func<CompletedTestCase, bool> shouldPrintPredicate);
}