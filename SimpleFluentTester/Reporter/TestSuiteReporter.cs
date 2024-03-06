using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Base class that should be used for defining your own custom reporter.
/// </summary>
internal sealed class TestSuiteReporter<TOutput>(TestSuiteResult<TOutput> testRunResult) : ITestSuiteReporter<TOutput>
{
    public void Report(Action<ITestSuiteReporterConfiguration<TOutput>, TestSuiteResult<TOutput>>? configurationBuilder = null)
    {
        var configuration = new TestSuiteReporterConfiguration<TOutput>();
        configurationBuilder?.Invoke(configuration, TestSuiteResult);
        
        var reportBuilder = configuration.ReportBuilder ?? new DefaultTestSuiteReportBuilder<TOutput>();
        var logger = configuration.Logger ?? CreateDefaultLogger();;
        try
        {
            var printableResult = reportBuilder.TestSuiteResultToString(TestSuiteResult);
            if (printableResult == null)
                return;
            logger.Log(printableResult.LogLevel, printableResult.EventId, null, printableResult.Message);
        }
        catch (Exception e)
        {
            logger.LogError(TestSuiteResult.Number, e, "Couldn't report a result of {number} TestSuite", TestSuiteResult.Number);
        }
    }

    public TestSuiteResult<TOutput> TestSuiteResult { get; } = testRunResult;

    private ILogger CreateDefaultLogger()
    {
        var loggerFactory = LoggerFactory.Create(loggerBuilder =>
        {
            loggerBuilder.ClearProviders();
            loggerBuilder.AddSimpleConsole(x =>
            {
                x.TimestampFormat = "HH:mm:ss.fff ";
                x.IncludeScopes = true;
                x.ColorBehavior = LoggerColorBehavior.Enabled;
            });
        });

        return loggerFactory.CreateLogger(TestSuiteResult.DisplayName ?? GetType().Name);
    }
}