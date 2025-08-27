using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Reporter.Console;

internal sealed class ConsoleTestSuiteReporter(ITestSuiteRunResult testSuiteRunResult) : ITestSuiteReporter
{
    public void Report(Action<ITestSuiteReporterConfigurationBuilder, ITestSuiteRunResult>? configurationBuilderInvoker = null)
    {
        var configurationBuilder = new ConsoleTestSuiteReporterConfigurationBuilder();
        configurationBuilderInvoker?.Invoke(configurationBuilder, TestSuiteRunResult);
        var configuration = (ConsoleTestSuiteReporterConfiguration) configurationBuilder.Build();
        
        if (configuration.LoggingBuilder == null)
            throw new InvalidOperationException("Even default logging builder was not specified, should be a bug.");

        var factory = LoggerFactory.Create(configuration.LoggingBuilder);
        var logger = factory.CreateLogger(TestSuiteRunResult.DisplayName ?? GetType().Name);

        try
        {
            var printableResult = configuration.ReportBuilder?.TestSuiteResultToString(TestSuiteRunResult, configuration.PrintablePredicate);
            if (printableResult == null)
                return;

            logger.Log(printableResult.LogLevel, printableResult.EventId, null, printableResult.Message);
        }
        catch (Exception e)
        {
            logger.LogError(new EventId(TestSuiteRunResult.Number), e, "Couldn't report a result of {number} TestSuite", TestSuiteRunResult.Number);
        }
        finally
        {
            factory.Dispose();
        }
    }

    public ITestSuiteRunResult TestSuiteRunResult { get; } = testSuiteRunResult;
}