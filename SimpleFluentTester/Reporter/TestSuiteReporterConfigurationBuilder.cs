using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

internal sealed class TestSuiteReporterConfigurationBuilder : ITestSuiteReporterConfigurationBuilder
{
    private readonly ITestSuiteReporterConfiguration _configuration = new TestSuiteReporterConfiguration();

    public ITestSuiteReporterConfigurationBuilder WithReportBuilder(Func<ITestSuiteReportBuilder> builderFactory)
    {
        _configuration.ReportBuilder = builderFactory.Invoke();
        return this;
    }

    public  ITestSuiteReporterConfigurationBuilder WithLoggingBuilder(Action<ILoggingBuilder> loggingBuilder)
    {
        _configuration.LoggingBuilder = loggingBuilder;
        return this;
    }

    public ITestSuiteReporterConfigurationBuilder WithPrintablePredicate(Func<CompletedTestCase, bool> printablePredicate)
    {
        _configuration.PrintablePredicate = printablePredicate;
        return this;
    }

    public ITestSuiteReporterConfiguration Build()
    {
        _configuration.ReportBuilder ??= new DefaultTestSuiteReportBuilder();
        _configuration.LoggingBuilder ??= DefaultLoggingBuilder;
        _configuration.PrintablePredicate ??= DefaultPrintablePredicate;
        
        return _configuration;
    }
    
    private static Action<ILoggingBuilder> DefaultLoggingBuilder
    {
        get
        {
            return loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSimpleConsole(x =>
                {
                    x.TimestampFormat = "HH:mm:ss.fff ";
                    x.IncludeScopes = true;
                    x.ColorBehavior = LoggerColorBehavior.Enabled;
                });
            };
        }
    }
    

    private static Func<CompletedTestCase, bool> DefaultPrintablePredicate
    {
        get
        {
            return testCase =>
            {
                var notPassed = testCase.Assert.Status == AssertStatus.NotPassed;
                var notPassedWithException = testCase.Assert.Status == AssertStatus.NotPassedWithException;
                var failed = testCase.Assert.Status == AssertStatus.Failed;
                var notValid = !testCase.Validation.IsValid;
                return notPassed || notPassedWithException || notValid || failed;
            };
        }
    }
}