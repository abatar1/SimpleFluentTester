using System.Collections.Generic;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public sealed record ValidatedObject(IComparedObject Object, List<ValidationResult>? ValidationResult = null)
{
    public IComparedObject Object { get; } = Object;
        
    public List<ValidationResult> ValidationResult { get; } = ValidationResult ?? [];
}