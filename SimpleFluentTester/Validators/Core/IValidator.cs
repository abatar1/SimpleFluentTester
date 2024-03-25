using System;
using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

public interface IValidator
{
    string Key { get; }
    
    ISet<Type> AllowedTypes { get; }
    
    ValidationSubject Subject { get; }
    
    ValidationResult Validate(IValidated validated, IValidatedObject validatedObject);
}