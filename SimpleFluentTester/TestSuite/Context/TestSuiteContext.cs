using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite.Context;

internal sealed class TestSuiteContext(
    int number,
    string name,
    IEntryAssemblyProvider entryAssemblyProvider,
    IActivator activator,
    IList<DefinedTestCase> testCases,
    Delegate? operation,
    Delegate? comparer,
    bool shouldBeExecuted) : ITestSuiteContext
{
    public int Number { get; } = number;
    
    public string Name { get; } = name;
    
    public bool ShouldBeExecuted { get; } = shouldBeExecuted;

    public IEntryAssemblyProvider EntryAssemblyProvider { get; } = entryAssemblyProvider;

    public IActivator Activator { get; } = activator;

    public IList<DefinedTestCase> TestCases { get; } = testCases;
    
    public Delegate? Operation { get; } = operation;
    
    public Delegate? Comparer { get; } = comparer;
}