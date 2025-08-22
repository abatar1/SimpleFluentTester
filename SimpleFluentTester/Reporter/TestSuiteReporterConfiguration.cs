using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;

namespace SimpleFluentTester.Reporter;

internal sealed class TestSuiteReporterConfiguration : ITestSuiteReporterConfiguration
{
    public ITestSuiteReportBuilder? ReportBuilder { get; set; }
    
    public Action<ILoggingBuilder>? LoggingBuilder { get; set; }
    
    public Func<AssertedTestClause, AssertedTestCase, bool>? PrintablePredicate { get; set; }
}