using System;
using System.Collections.Generic;
using SimpleFluentTester.Entities;
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
        return new TestRunBuilder<TOutput>(GetDefaultContext(true, comparer, null));
    }
    
    /// <summary>
    /// Specifies the method that needs to be tested without defining of expected return type of delegate.
    /// However, each time you set an expected result, the expected result type will be checked. 
    /// </summary>
    public static TestRunBuilder<object> UseOperation(Delegate operation)
    {
        return new TestRunBuilder<object>(GetDefaultContext<object>(true, null, operation));
    }

    /// <summary>
    /// Add this call if you want your test suite to be ignored instead of commenting it, useful when you have multiple
    /// test cases in a single project.
    /// </summary>
    public static IgnoredTestRunBuilder Ignore => new();
    
    public class IgnoredTestRunBuilder
    {
        /// <summary>
        /// This method fakes <see cref="TestSuite.WithExpectedReturnType{TOutput}"/> declaration just to keep fluent flow of methods.
        /// </summary>
        public TestRunBuilder<TOutput> WithExpectedReturnType<TOutput>(Func<TOutput, TOutput, bool>? comparer = null)
        {
            return new TestRunBuilder<TOutput>(GetDefaultContext(false, comparer, null));
        }
    }

    private static TestRunBuilderContext<TOutput> GetDefaultContext<TOutput>(bool shouldBeExecuted, Func<TOutput, TOutput, bool>? comparer, Delegate? operation)
    {
        return new TestRunBuilderContext<TOutput>(
            new EntryAssemblyProvider(), 
            new DefaultActivator(),
            new List<TestCase<TOutput>>(), 
            new DefaultTestRunReporterFactory(), 
            new ValueWrapper<Delegate>(operation), 
            comparer,
            shouldBeExecuted);
    }
}