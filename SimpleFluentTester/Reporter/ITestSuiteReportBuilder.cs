using System;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Converts <see cref="TestSuiteRunResult"/> to printable formatted string.
/// </summary>
public interface ITestSuiteReportBuilder
{
    PrintableTestSuiteResult? TestSuiteResultToString(
        TestSuiteRunResult testSuiteRunResult,
        Func<AssertedTestClause, AssertedTestCase, bool>? shouldPrintPredicate);
}