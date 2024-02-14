using System;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester;

public class IgnoredTestRunBuilder
{
    /// <summary>
    /// This method fakes <see cref="TestSuite.WithExpectedReturnType{TOutput}"/> declaration just to keep fluent flow of methods.
    /// </summary>
    public TestRunBuilder<TOutput> WithExpectedReturnType<TOutput>(Func<TOutput, TOutput, bool>? comparer = null)
    {
        return new TestRunBuilder<TOutput>(new DefaultTestRunReporterFactory(), new EntryAssemblyProvider(), new DefaultActivator(), comparer, false);
    }
}