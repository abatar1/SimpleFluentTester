using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.TestCase;

public interface ITestCaseBuilder<TOutput>
{
    ITestSuiteBuilder<TOutput> WithInput(params object?[] inputs);
}