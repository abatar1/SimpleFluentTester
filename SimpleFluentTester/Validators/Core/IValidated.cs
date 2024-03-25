using System;
using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

public interface IValidated
{
    IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; }
}