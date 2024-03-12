using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.Reporter;

public interface ITestSuiteReporterConfiguration<TOutput>
{
    ITestSuiteReportBuilder<TOutput>? ReportBuilder { get; set; }
    
    ILogger? Logger { get; set; }
    
    Func<CompletedTestCase<TOutput>, bool>? ShouldPrintPredicate { get; set; }
}