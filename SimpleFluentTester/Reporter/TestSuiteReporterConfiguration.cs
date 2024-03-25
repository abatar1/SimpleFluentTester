using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

/// <inheritdoc cref="ITestSuiteReporterConfiguration"/>
internal sealed class TestSuiteReporterConfiguration : ITestSuiteReporterConfiguration
{
    /// <inheritdoc cref="ITestSuiteReporterConfiguration.ReportBuilder"/>
    public ITestSuiteReportBuilder? ReportBuilder { get; set; }
    
    /// <inheritdoc cref="ITestSuiteReporterConfiguration.Logger"/>
    public ILogger? Logger { get; set; }
    
    /// <inheritdoc cref="ITestSuiteReporterConfiguration.ShouldPrintPredicate"/>
    public Func<CompletedTestCase, bool>? ShouldPrintPredicate { get; set; }
}