using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

public sealed class TestCase(object?[] inputs, IComparedObject expected, int number) : IValidated
{
    public object?[] Inputs { get; } = inputs;
    
    public IComparedObject Expected { get; } = expected;
    
    public int Number { get; } = number;

    public IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; } =
        new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>();
}