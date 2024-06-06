using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.Reporter;

public interface ITestSuiteReporterConfigurationBuilder
{
    ITestSuiteReporterConfigurationBuilder WithReportBuilder(Func<ITestSuiteReportBuilder> builderFactory);

    ITestSuiteReporterConfigurationBuilder WithLoggingBuilder(Action<ILoggingBuilder> loggingBuilder);

    ITestSuiteReporterConfigurationBuilder WithPrintablePredicate(Func<CompletedTestCase, bool> printablePredicate);

    ITestSuiteReporterConfiguration Build();
}