using System;
using SimpleTester.Core.Reporter;
using SimpleTester.Core.TestRun;

namespace SimpleTester.Core.TestSuite;

public sealed class TestSuiteCustomBuilder
{
    private BaseTestRunReporterFactory? _reporterFactory;
    
    public TestSuiteCustomBuilder WithCustomReporterFactory<TReporterFactory>()
        where TReporterFactory : BaseTestRunReporterFactory
    {
        _reporterFactory = (TReporterFactory)Activator.CreateInstance(typeof(TReporterFactory));
        return this;
    }

    public TestRunOperationBuilder Setup() => new(_reporterFactory ?? new DefaultTestRunReporterFactory());
}