using System;
using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

public interface IValidatedObject
{
    IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> Validations { get; }
}