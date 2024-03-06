using SimpleFluentTester.Suite;

namespace SimpleFluentTester.Reporter
{
    /// <summary>
    /// Factory that needs to be inherited to return your custom reporter.
    /// </summary>
    public interface ITestSuiteReporterFactory
    {
        ITestSuiteReporter GetReporter<TOutput>(TestSuiteResult<TOutput> testRunResult);
    }
}