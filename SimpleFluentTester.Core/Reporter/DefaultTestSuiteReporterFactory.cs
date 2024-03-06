using SimpleFluentTester.Suite;

namespace SimpleFluentTester.Reporter
{
    internal sealed class DefaultTestSuiteReporterFactory : ITestSuiteReporterFactory
    {
        public ITestSuiteReporter GetReporter<TOutput>(TestSuiteResult<TOutput> testRunResult)
        {
            return new DefaultTestSuiteReporter<TOutput>(testRunResult);
        }
    }
}