using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

/// <inheritdoc cref="ITestSuiteReporterConfiguration"/>
internal sealed class TestSuiteReporterConfiguration : ITestSuiteReporterConfiguration
{
    /// <inheritdoc cref="ITestSuiteReporterConfiguration.ReportBuilder"/>
    public ITestSuiteReportBuilder? ReportBuilder { get; set; }

    /// <inheritdoc cref="ITestSuiteReporterConfiguration.LoggingBuilder"/>
    public Action<ILoggingBuilder>? LoggingBuilder { get; set; }

    /// <inheritdoc cref="ITestSuiteReporterConfiguration.PrintablePredicate"/>
    public Func<CompletedTestCase, bool>? PrintablePredicate { get; set; }
}