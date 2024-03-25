using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Context;

internal sealed class TestSuiteContext(
    int number,
    string name,
    IEntryAssemblyProvider entryAssemblyProvider,
    IActivator activator,
    IList<TestCase> testCases,
    Delegate? operation,
    Delegate? comparer,
    IDictionary<ValidationSubject, IList<Func<ValidationResult>>> validations,
    bool shouldBeExecuted) : ITestSuiteContext
{
    public int Number { get; } = number;
    
    public string Name { get; } = name;
    
    public bool ShouldBeExecuted { get; } = shouldBeExecuted;
    
    public IEntryAssemblyProvider EntryAssemblyProvider { get; } = entryAssemblyProvider;

    public IActivator Activator { get; } = activator;

    public IList<TestCase> TestCases { get; } = testCases;
    
    public Delegate? Operation { get; } = operation;
    
    public Delegate? Comparer { get; } = comparer;

    public IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; } = validations;
}