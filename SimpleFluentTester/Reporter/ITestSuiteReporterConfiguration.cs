using System;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Allows to configure <see cref="ITestSuiteReporter"/>
/// </summary>
public interface ITestSuiteReporterConfiguration
{
    /// <summary>
    /// Allows setting a custom string report builder; otherwise the default builder will be used.
    /// </summary>
    ITestSuiteReportBuilder? ReportBuilder { get; set; }
    
    /// <summary>
    /// Allows specifying predicate that defines should a test case be printed or not; by default, only failed test cases will be printed.
    /// </summary>
    Func<AssertedTestClause, AssertedTestCase, bool>? PrintablePredicate { get; set; }
}