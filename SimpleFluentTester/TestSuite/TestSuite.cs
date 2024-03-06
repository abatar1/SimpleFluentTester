using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Starting point used to initialize tests.
/// </summary>
public static class TestSuite
{
    private static int _testSuiteNumber;
        
    public static ITestSuiteBuilder<object> Sequential
    {
        get
        {
            _testSuiteNumber += 1;
            var context = new TestSuiteBuilderContext<object>(
                _testSuiteNumber,
                nameof(TestSuite),
                new EntryAssemblyProvider(), 
                new DefaultActivator(),
                new List<TestCase<object>>(), 
                new ValueWrapper<Delegate>(), 
                null,
                true);
            return new TestSuiteBuilder<object>(context);
        }
    }
}