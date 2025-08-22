using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

public interface IValidator
{
    string Key { get; }
    
    ValidationSubject Subject { get; }
    
    SubjectValidation Validate(IValidatedObject validated, IValidationContext validationContext);
}