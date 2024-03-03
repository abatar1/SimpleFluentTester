using System;
using System.Collections.Generic;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester;

/// <summary>
/// Starting point used to initialize tests.
/// </summary>
public static class TestSuite
{
    public static TestRunBuilder<object> Sequential
    {
        get
        {
            var context = new TestRunBuilderContext<object>(
                new EntryAssemblyProvider(), 
                new DefaultActivator(),
                new List<TestCase<object>>(), 
                new HashSet<ValidationInvoker<object>>(),
                new DefaultTestRunReporterFactory(), 
                new ValueWrapper<Delegate>(), 
                null,
                true);
            return new TestRunBuilder<object>(context);
        }
    }
}