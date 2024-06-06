using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Reporter;

internal sealed class TestSuiteReporter(TestSuiteRunResult testSuiteRunResult) : ITestSuiteReporter
{
    public void Report(Action<ITestSuiteReporterConfigurationBuilder, TestSuiteRunResult>? configurationBuilderInvoker = null)
    {
        var configurationBuilder = new TestSuiteReporterConfigurationBuilder();
        configurationBuilderInvoker?.Invoke(configurationBuilder, TestSuiteRunResult);
        var configuration = configurationBuilder.Build();
        
        if (configuration.LoggingBuilder == null)
            throw new InvalidOperationException("Even default logging builder was not specified, should be a bug.");

        var logger = LoggerFactory.Create(configuration.LoggingBuilder).CreateLogger(TestSuiteRunResult.DisplayName ?? GetType().Name);
        
        try
        {
            var printableResult = configuration.ReportBuilder?.TestSuiteResultToString(TestSuiteRunResult, configuration.PrintablePredicate);
            if (printableResult == null)
                return;
          
            logger.Log(printableResult.LogLevel, printableResult.EventId, null, printableResult.Message);
        }
        catch (Exception e)
        {
            logger.LogError(new EventId(TestSuiteRunResult.Number), e, "Couldn't report a result of {number} TestSuite",
                TestSuiteRunResult.Number);
        }
    }

    public TestSuiteRunResult TestSuiteRunResult { get; } = testSuiteRunResult;
}