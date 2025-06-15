using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

internal sealed class TestSuiteReporterConfiguration : ITestSuiteReporterConfiguration
{
    public ITestSuiteReportBuilder? ReportBuilder { get; set; }
    
    public Action<ILoggingBuilder>? LoggingBuilder { get; set; }
    
    public Func<AssertedTestCase, bool>? PrintablePredicate { get; set; }
}