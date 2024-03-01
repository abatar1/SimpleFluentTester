using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Examples;

internal sealed class CustomReporterFactory : ITestRunReporterFactory
{
    public ITestRunReporter GetReporter<TOutput>(TestRunResult<TOutput> testRunResult)
    {
        return new CustomReporter<TOutput>(testRunResult);
    }
}