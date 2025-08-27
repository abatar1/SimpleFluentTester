using System;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;

namespace SimpleFluentTester.Reporter;

public interface ITestSuiteReporterConfigurationBuilder
{
    ITestSuiteReporterConfigurationBuilder WithReportBuilder<TTestSuiteReportBuilder>()
        where TTestSuiteReportBuilder : class, ITestSuiteReportBuilder;

    ITestSuiteReporterConfigurationBuilder WithPrintablePredicate(Func<AssertedTestClause, AssertedTestCase, bool> printablePredicate);

    ITestSuiteReporterConfiguration Build();
}