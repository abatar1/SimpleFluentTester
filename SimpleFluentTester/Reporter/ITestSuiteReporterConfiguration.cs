using Microsoft.Extensions.Logging;

namespace SimpleFluentTester.Reporter;

public interface ITestSuiteReporterConfiguration<TOutput>
{
    ITestSuiteReportBuilder<TOutput>? ReportBuilder { get; set; }
    
    ILogger? Logger { get; set; }
}