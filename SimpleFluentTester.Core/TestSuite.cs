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
        if (!typeof(IEquatable<TOutput>).IsAssignableFrom(typeof(TOutput)) && comparer == null)
            throw new InvalidOperationException("TOutput type should be assignable from IEquatable<TOutput> or comparer should be defined");
        
        return new TestRunBuilder<TOutput>(new DefaultTestRunReporterFactory(), new EntryAssemblyProvider(), comparer);
    }
}