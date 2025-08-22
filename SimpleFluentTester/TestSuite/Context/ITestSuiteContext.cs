using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite.Context;

internal interface ITestSuiteContext
{
    int Number { get; }
    
    string Name { get; }
    
    bool ShouldBeExecuted { get; }
    
    IEntryAssemblyProvider EntryAssemblyProvider { get; }

    IActivator Activator { get; }

    IList<DefinedTestCase> TestCases { get; }

    Delegate? Operation { get; }
    
    Delegate? Comparer { get; }
}