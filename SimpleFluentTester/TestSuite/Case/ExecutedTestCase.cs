using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

public sealed class ExecutedTestCase(
    IComparedObject result, 
    IComparedObject expected,
    TimeSpan elapsedTime,
    Delegate comparer,
    ITestCase testCase,
    IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> validations
    ) : ITestCase
{
    public IComparedObject Result { get; } = result;

    public int Number { get; } = testCase.Number;
    
    public IComparedObject[] Inputs { get; } = testCase.Inputs;
    
    public IComparedObject Expected { get; } = expected;
    
    public Delegate Comparer { get; } = comparer;

    public TimeSpan ElapsedTime { get; } = elapsedTime;

    public IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> Validations { get; } = validations;
}