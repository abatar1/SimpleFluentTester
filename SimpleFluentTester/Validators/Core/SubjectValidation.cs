using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

public record SubjectValidation(IList<ValidationResult> Validations)
{
    public IList<ValidationResult> Validations { get; } = Validations;
}