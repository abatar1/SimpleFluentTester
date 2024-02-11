using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester;

/// <summary>
/// Starting point used to initialize tests.
/// </summary>
public static class TestSuite
{
    /// <summary>
    /// Defines the return type of the function that we plan to test.
    /// </summary>
    public static TestRunBuilder<TOutput> WithExpectedReturnType<TOutput>()
    {
        return new TestRunBuilder<TOutput>(new DefaultTestRunReporterFactory(), new EntryAssemblyProvider());
    }
}