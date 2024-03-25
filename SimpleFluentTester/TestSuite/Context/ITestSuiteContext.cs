using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Context;

public interface ITestSuiteContext : IValidated
{
    int Number { get; }
    
    string Name { get; }
    
    bool ShouldBeExecuted { get; }
    
    IEntryAssemblyProvider EntryAssemblyProvider { get; }

    IActivator Activator { get; }

    IList<TestCase> TestCases { get; }

    Delegate? Operation { get; }
    
    Delegate? Comparer { get; }
}