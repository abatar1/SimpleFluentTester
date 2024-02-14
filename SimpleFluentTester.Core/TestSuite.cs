using System;
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
    /// The type should implement IEquatable interface or comparer should be provided. 
    /// </summary>
    public static TestRunBuilder<TOutput> WithExpectedReturnType<TOutput>(Func<TOutput, TOutput, bool>? comparer = null)
    {
        return new TestRunBuilder<TOutput>(new DefaultTestRunReporterFactory(), new EntryAssemblyProvider(), new DefaultActivator(), comparer);
    }

    /// <summary>
    /// Add this call if you want your test suite to be ignored instead of commenting it, useful when you have multiple
    /// test cases in a single project.
    /// </summary>
    public static IgnoredTestRunBuilder Ignore => new();
}