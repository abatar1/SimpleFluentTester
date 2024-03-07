using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Context;

public interface ITestSuiteBuilderContext<TOutput>
{
    int Number { get; }
    
    string Name { get; }
    
    bool ShouldBeExecuted { get; }
    
    IEntryAssemblyProvider EntryAssemblyProvider { get; }

    IActivator Activator { get; }

    IList<TestCase<TOutput>> TestCases { get; }
    
    IDictionary<ValidationSubject, IList<ValidationResult>> Validations { get; }

    Delegate? Operation { get; }
    
    Func<TOutput?, TOutput?, bool>? Comparer { get; }
    
    Type? OutputUnderlyingType { get; }
}