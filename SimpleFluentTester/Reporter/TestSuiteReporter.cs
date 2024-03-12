using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Reporter;

internal sealed class TestSuiteReporter<TOutput>(TestSuiteResult<TOutput> testRunResult) : ITestSuiteReporter<TOutput>
{
    public void Report(Action<ITestSuiteReporterConfiguration<TOutput>, TestSuiteResult<TOutput>>? configurationBuilder = null)
    {
        var configuration = new TestSuiteReporterConfiguration<TOutput>();
        configurationBuilder?.Invoke(configuration, TestSuiteResult);

        var reportBuilder = configuration.ReportBuilder ?? new DefaultTestSuiteReportBuilder<TOutput>();
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
            logger.LogError(TestSuiteResult.Number, e, "Couldn't report a result of {number} TestSuite",
                TestSuiteResult.Number);
        }
    }

    public TestSuiteResult<TOutput> TestSuiteResult { get; } = testRunResult;

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

    private static Func<CompletedTestCase<TOutput>, bool> DefaultShouldPrintPredicate
    {
        get
        {
            return testCase =>
            {
                var notPassed = testCase.Assert?.Status == AssertStatus.NotPassed;
                var notPassedWithException = testCase.Assert?.Status == AssertStatus.NotPassedWithException;
                var notValid = testCase.ValidationStatus != ValidationStatus.Valid;
                return notPassed || notPassedWithException || notValid;
            };
        }
    }
}