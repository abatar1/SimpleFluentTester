using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.Reporter;

internal sealed class TestSuiteReporterConfiguration<TOutput> : ITestSuiteReporterConfiguration<TOutput>
{
    public ITestSuiteReportBuilder<TOutput>? ReportBuilder { get; set; }
    
    public ILogger? Logger { get; set; }
    
    public Func<CompletedTestCase<TOutput>, bool>? ShouldPrintPredicate { get; set; }
}