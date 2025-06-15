using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.TestSuite.Context;

internal interface ITestSuiteContext
{
    int Number { get; }
    
    string Name { get; }
    
    bool ShouldBeExecuted { get; }
    
    IEntryAssemblyProvider EntryAssemblyProvider { get; }

    IActivator Activator { get; }

    IList<DeferredTestCase> TestCases { get; }

    Delegate? Operation { get; }
    
    Delegate? Comparer { get; }
}