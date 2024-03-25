using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Context;

internal sealed class TestSuiteContextContainer : ITestSuiteContextContainer
{
    public ITestSuiteContext Context { get; private set; }

    internal TestSuiteContextContainer(ITestSuiteContext context)
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
            Context.Validations,
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
            Context.Validations,
            Context.ShouldBeExecuted);
    }
    
    public void WithComparer(Delegate comparer)
    {
        Context = new TestSuiteContext(
            Context.Number,
            Context.Name,
            Context.EntryAssemblyProvider,
            Context.Activator,
            Context.TestCases,
            Context.Operation,
            comparer,
            Context.Validations,
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
            Context.Validations,
            false);
    }
        
    public static TestSuiteContextContainer Default(int testSuiteNumber)
    {
        var context = new TestSuiteContext(
            testSuiteNumber,
            nameof(TestSuite),
            new EntryAssemblyProvider(), 
            new DefaultActivator(),
            new List<TestCase>(), 
            null, 
            null,
            new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>(),
            true);
        return new TestSuiteContextContainer(context);
    }
}
