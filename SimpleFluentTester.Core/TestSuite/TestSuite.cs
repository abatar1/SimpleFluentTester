using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Suite
{
    /// <summary>
    /// Starting point used to initialize tests.
    /// </summary>
    public static class TestSuite
    {

        private static int _testSuiteNumber;
        
        public static TestSuiteBuilder<object> Sequential
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
                    new HashSet<ValidationInvoker<object>>(),
                    new DefaultTestSuiteReporterFactory(), 
                    new ValueWrapper<Delegate>(), 
                    null,
                    true);
                return new TestSuiteBuilder<object>(context);
            }
        }
    }
}