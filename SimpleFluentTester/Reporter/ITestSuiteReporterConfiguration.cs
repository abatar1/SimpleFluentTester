using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Allows to configure <see cref="ITestSuiteReporter"/>
/// </summary>
public interface ITestSuiteReporterConfiguration
{
    /// <summary>
    /// Allows to set custom string report builder; otherwise default builder will be used.
    /// </summary>
    ITestSuiteReportBuilder? ReportBuilder { get; set; }
    
    /// <summary>
    /// Allows to setup custom logger for reporter; otherwise default console logger will be used.
    /// </summary>
    ILogger? Logger { get; set; }
    
    /// <summary>
    /// Allows to specify predicate that defines should test case be printed or not; by default only failed test cases will be printed.
    /// </summary>
    Func<CompletedTestCase, bool>? ShouldPrintPredicate { get; set; }
}