using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite;

public interface ITestSuiteBuilderContext<TOutput>
{
    int Number { get; }
    
    string Name { get; set; }
    
    bool ShouldBeExecuted { get; set; }
    
    IEntryAssemblyProvider EntryAssemblyProvider { get; }

    IActivator Activator { get; }

    IList<TestCase<TOutput>> TestCases { get; }

    ValueWrapper<Delegate> Operation { get; }
    
    Func<TOutput?, TOutput?, bool>? Comparer { get; }
    
    bool IsObjectOutput { get; }
    
    Type? OutputUnderlyingType { get; }
    
    IReadOnlyCollection<ParameterInfo> OperationParameters { get; }
}