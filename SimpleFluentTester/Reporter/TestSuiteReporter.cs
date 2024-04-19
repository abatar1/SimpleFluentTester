using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

internal sealed class TestSuiteReporter(TestSuiteResult testRunResult) : ITestSuiteReporter
{
    public void Report(Action<ITestSuiteReporterConfiguration, TestSuiteResult>? configurationBuilder = null)
    {
        ITestSuiteReporterConfiguration configuration = new TestSuiteReporterConfiguration();
        configurationBuilder?.Invoke(configuration, TestSuiteResult);

        var reportBuilder = configuration.ReportBuilder ?? new DefaultTestSuiteReportBuilder();
        var logger = configuration.Logger ?? DefaultLogger;
        var shouldPrintPredicate = configuration.ShouldPrintPredicate ?? DefaultShouldPrintPredicate;
        try
        {
            var printableResult = reportBuilder.TestSuiteResultToString(TestSuiteResult, shouldPrintPredicate);
            if (printableResult == null)
                return;
            logger.Log(printableResult.LogLevel, printableResult.EventId, null, printableResult.Message);
        }
        catch (Exception e)
        {
            logger.LogError(new EventId(TestSuiteResult.Number), e, "Couldn't report a result of {number} TestSuite",
                TestSuiteResult.Number);
        }
    }

    public TestSuiteResult TestSuiteResult { get; } = testRunResult;

    private ILogger DefaultLogger
    {
        get
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

    private static Func<CompletedTestCase, bool> DefaultShouldPrintPredicate
    {
        get
        {
            return testCase =>
            {
                var notPassed = testCase.Assert.Status == AssertStatus.NotPassed;
                var notPassedWithException = testCase.Assert.Status == AssertStatus.NotPassedWithException;
                var notValid = !testCase.Validation.IsValid;
                return notPassed || notPassedWithException || notValid;
            };
        }
    }
}