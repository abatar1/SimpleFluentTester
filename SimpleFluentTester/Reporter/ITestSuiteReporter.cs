using System;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Reporter;

public interface ITestSuiteReporter<TOutput>
{
    /// <summary>
    /// Prints the results of test execution using the standard reporter or, if specified, using a custom reporter.
    /// </summary>
    void Report(Action<ITestSuiteReporterConfiguration<TOutput>, TestSuiteResult<TOutput>>? configurationBuilder = null);
    
    /// <summary>
    /// Represents test suite run results.
    /// </summary>
    TestSuiteResult<TOutput> TestSuiteResult { get; }
}