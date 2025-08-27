namespace SimpleFluentTester.Validators.Models;

public interface IValidator
{
    string Key { get; }
    
    ValidationSubject Subject { get; }
    
    SubjectValidation Validate(IValidatedObject validated, IValidationContext validationContext);
}