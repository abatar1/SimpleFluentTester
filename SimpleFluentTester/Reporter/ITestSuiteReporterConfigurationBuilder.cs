using System;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;

namespace SimpleFluentTester.Reporter;

public interface ITestSuiteReporterConfigurationBuilder
{
    ITestSuiteReporterConfigurationBuilder WithReportBuilder(Func<ITestSuiteReportBuilder> builderFactory);

    ITestSuiteReporterConfigurationBuilder WithLoggingBuilder(Action<ILoggingBuilder> loggingBuilder);

    ITestSuiteReporterConfigurationBuilder WithPrintablePredicate(Func<AssertedTestClause, AssertedTestCase, bool> printablePredicate);

    ITestSuiteReporterConfiguration Build();
}