using System;

namespace SimpleFluentTester.Validators.Core;

public interface IValidator
{
    string Key { get; }
    
    Type AllowedType { get; }
    
    ValidationSubject Subject { get; }
    
    ValidationResult Validate(IValidatedObject validated, IValidationContext validationContext);
}