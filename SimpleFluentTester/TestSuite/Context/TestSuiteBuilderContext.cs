using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Context;

internal sealed class TestSuiteBuilderContext<TOutput>(
    int number,
    string name,
    IEntryAssemblyProvider entryAssemblyProvider,
    IActivator activator,
    IList<TestCase<TOutput>> testCases,
    Delegate? operation,
    Func<TOutput?, TOutput?, bool>? comparer,
    IDictionary<ValidationSubject, IList<ValidationResult>> validations,
    bool shouldBeExecuted,
    Type? outputUnderlyingType = null) : ITestSuiteBuilderContext<TOutput>
{
    public int Number { get; } = number;
    
    public string Name { get; } = name;
    
    public bool ShouldBeExecuted { get; } = shouldBeExecuted;
    
    public IEntryAssemblyProvider EntryAssemblyProvider { get; } = entryAssemblyProvider;

    public IActivator Activator { get; } = activator;

    public IList<TestCase<TOutput>> TestCases { get; } = testCases;

    public IDictionary<ValidationSubject, IList<ValidationResult>> Validations { get; } = validations;

    public Delegate? Operation { get; } = operation;
    
    public Func<TOutput?, TOutput?, bool>? Comparer { get; } = comparer;

    public Type? OutputUnderlyingType
    {
        get
        {
            if (outputUnderlyingType != null)
                return outputUnderlyingType;

            if (Operation?.Method.ReturnParameter == null)
                return null;

            outputUnderlyingType = Nullable.GetUnderlyingType(Operation.Method.ReturnParameter.ParameterType);
            return outputUnderlyingType;
        }
    }
    
    public static ITestSuiteBuilderContext<TOutput> Default(int testSuiteNumber)
    {
        return new TestSuiteBuilderContext<TOutput>(
            testSuiteNumber,
            nameof(TestSuite),
            new EntryAssemblyProvider(), 
            new DefaultActivator(),
            new List<TestCase<TOutput>>(), 
            null, 
            null,
            new Dictionary<ValidationSubject, IList<ValidationResult>>(),
            true);
    }
}