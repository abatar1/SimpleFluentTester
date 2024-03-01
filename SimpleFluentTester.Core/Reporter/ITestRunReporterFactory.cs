using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Factory that needs to be inherited to return your custom reporter.
/// </summary>
public interface ITestRunReporterFactory
{
    ITestRunReporter GetReporter<TOutput>(TestRunResult<TOutput> testRunResult);
}