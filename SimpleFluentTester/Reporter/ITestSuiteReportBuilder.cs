using System;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Converts <see cref="TestSuiteRunResult"/> to printable formatted string.
/// </summary>
public interface ITestSuiteReportBuilder
{
    PrintableTestSuiteResult? TestSuiteResultToString(
        TestSuiteRunResult testSuiteRunResult,
        Func<CompletedTestCase, bool>? shouldPrintPredicate);
}