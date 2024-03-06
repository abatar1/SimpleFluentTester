using Microsoft.Extensions.Logging;

namespace SimpleFluentTester.Reporter;

internal sealed class TestSuiteReporterConfiguration<TOutput> : ITestSuiteReporterConfiguration<TOutput>
{
    public ITestSuiteReportBuilder<TOutput>? ReportBuilder { get; set; }
    
    public ILogger? Logger { get; set; }
}