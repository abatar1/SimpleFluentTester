using System;
using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

public interface IValidatedObject
{
    IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; }
}

public sealed class EmptyValidatedObject : IValidatedObject
{
    public IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; } =
        new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>();
}