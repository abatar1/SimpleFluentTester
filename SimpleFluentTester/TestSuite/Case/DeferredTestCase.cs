using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

/// <summary>
/// Represents a deferred test case that is constructed with lazily evaluated operations and comparers.
/// </summary>
public sealed class DeferredTestCase(
    Lazy<Delegate?> operationFactory,
    Lazy<Delegate?> comparerFactory,
    IComparedObject[] inputs,
    IComparedObject expected,
    int number) : ITestCase
{
    public int Number { get; } = number;
    
    public IComparedObject[] Inputs { get; } = inputs;
    
    public IComparedObject Expected { get; } = expected;

    public Lazy<Delegate?> OperationFactory { get; } = operationFactory;
    
    public Lazy<Delegate?> ComparerFactory { get; } = comparerFactory;
    
    public IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> Validations { get; } =
        new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>>();
    
    
}