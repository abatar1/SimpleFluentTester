﻿using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

public sealed class TestCase(IComparedObject[] inputs, IComparedObject expected, int number) : IValidated
{
    public IComparedObject[] Inputs { get; } = inputs;
    
    public IComparedObject Expected { get; } = expected;
    
    public int Number { get; } = number;

    public IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; } =
        new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>();
}