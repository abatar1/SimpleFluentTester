using SimpleFluentTester.Reporter;
using SimpleFluentTester.Suite;

namespace SimpleFluentTester.Examples
{
    internal sealed class CustomReporterFactory : ITestSuiteReporterFactory
    {
        public ITestSuiteReporter GetReporter<TOutput>(TestSuiteResult<TOutput> testRunResult)
        {
            return new CustomReporter<TOutput>(testRunResult);
        }
    }
}