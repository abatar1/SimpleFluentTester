using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite.Context;

internal sealed class TestSuiteContextContainer : ITestSuiteContextContainer
{
    public ITestSuiteContext Context { get; private set; }

    private TestSuiteContextContainer(ITestSuiteContext context)
    {
        Context = context;
    }
    
    public void WithOperation(Delegate operation)
    {
        Context = new TestSuiteContext(
            Context.Number,
            Context.Name,
            Context.EntryAssemblyProvider,
            Context.Activator,
            Context.TestCases,
            operation,
            Context.Comparer,
            Context.ShouldBeExecuted);
    }
    
    public void WithDisplayName(string displayName)
    {
        Context = new TestSuiteContext(
            Context.Number,
            displayName,
            Context.EntryAssemblyProvider,
            Context.Activator,
            Context.TestCases,
            Context.Operation,
            Context.Comparer,
            Context.ShouldBeExecuted);
    }
    
    public void WithComparer<TExpected>(ComparerDelegate<TExpected> comparer)
    {
        Context = new TestSuiteContext(
            Context.Number,
            Context.Name,
            Context.EntryAssemblyProvider,
            Context.Activator,
            Context.TestCases,
            Context.Operation,
            comparer,
            Context.ShouldBeExecuted);
    }
    
    public void DoNotExecute()
    {
        Context = new TestSuiteContext(
            Context.Number,
            Context.Name,
            Context.EntryAssemblyProvider,
            Context.Activator,
            Context.TestCases,
            Context.Operation,
            Context.Comparer,
            false);
    }
    
    public void WithEntryAssemblyProvider(IEntryAssemblyProvider entryAssemblyProvider)
    {
        Context = new TestSuiteContext(
            Context.Number,
            Context.Name,
            entryAssemblyProvider,
            Context.Activator,
            Context.TestCases,
            Context.Operation,
            Context.Comparer,
            Context.ShouldBeExecuted);
    }
    
    public void WithActivator(IActivator activator)
    {
        Context = new TestSuiteContext(
            Context.Number,
            Context.Name,
            Context.EntryAssemblyProvider,
            activator,
            Context.TestCases,
            Context.Operation,
            Context.Comparer,
            Context.ShouldBeExecuted);
    }
        
    public static TestSuiteContextContainer Default(int testSuiteNumber = 1)
    {
        var context = new TestSuiteContext(
            testSuiteNumber,
            nameof(TestSuite),
            new EntryAssemblyProvider(), 
            new DefaultActivator(),
            new List<DefinedTestCase>(), 
            null, 
            null,
            true);
        return new TestSuiteContextContainer(context);
    }
}
