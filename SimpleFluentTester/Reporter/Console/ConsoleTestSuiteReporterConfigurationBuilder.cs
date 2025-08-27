using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Helpers;

namespace SimpleFluentTester.Reporter.Console;

internal sealed class ConsoleTestSuiteReporterConfigurationBuilder : ITestSuiteReporterConfigurationBuilder
{
    private readonly ConsoleTestSuiteReporterConfiguration _configuration = new();

    public ITestSuiteReporterConfigurationBuilder WithReportBuilder<TTestSuiteReportBuilder>() 
        where TTestSuiteReportBuilder : class, ITestSuiteReportBuilder
    {
        _configuration.ReportBuilder = Activator.CreateInstance<TTestSuiteReportBuilder>();
        return this;
    }

    public ITestSuiteReporterConfigurationBuilder WithPrintablePredicate(Func<AssertedTestClause, AssertedTestCase, bool> printablePredicate)
    {
        _configuration.PrintablePredicate = printablePredicate;
        return this;
    }

    public ITestSuiteReporterConfiguration Build()
    {
        _configuration.ReportBuilder ??= new ConsoleTestSuiteReportBuilder();
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
    

    private static Func<AssertedTestClause, AssertedTestCase, bool> DefaultPrintablePredicate
    {
        get
        {
            return (clause, testCase) =>
            {
                var notPassed = clause.Assert.Status == AssertStatus.NotPassed;
                var notPassedWithException = clause.Assert.Status == AssertStatus.NotPassedWithException;
                var failed = clause.Assert.Status == AssertStatus.Failed;
                var notValid = !testCase.IsValid();
                return notPassed || notPassedWithException || notValid || failed;
            };
        }
    }
}