using System;
using SimpleFluentTester.Reporter.Console;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Converts <see cref="TestSuiteRunResult"/> to printable formatted string.
/// </summary>
public interface ITestSuiteReportBuilder
{
    ConsoleTestSuiteResult? TestSuiteResultToString(
        ITestSuiteRunResult testSuiteRunResult,
        Func<AssertedTestClause, AssertedTestCase, bool>? shouldPrintPredicate);
}