using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestRunReporterFactory : ITestRunReporterFactory
{
    public ITestRunReporter GetReporter<TOutput>(TestSuiteResult<TOutput> testRunResult)
    {
        return new DefaultTestRunReporter<TOutput>(testRunResult);
    }
}