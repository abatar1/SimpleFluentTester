using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

/// <summary>
/// Represents not yet processed test case. 
/// </summary>
public sealed class TestCase(
    Func<Delegate?> operationFactory, 
    Func<Delegate?> comparerFactory, 
    IComparedObject[] inputs,
    IComparedObject expected, 
    int number) : IValidatedObject
{
    public int Number { get; } = number;
    
    public IComparedObject[] Inputs { get; } = inputs;
    
    public IComparedObject Expected { get; } = expected;

    public Func<Delegate?> OperationFactory { get; } = operationFactory;
    
    public Func<Delegate?> ComparerFactory { get; } = comparerFactory;

    public IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; } =
        new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>();
}