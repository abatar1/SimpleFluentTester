namespace SimpleFluentTester.Validators.Core;

public interface IValidator
{
    string Key { get; }
    
    ValidationSubject Subject { get; }
    
    ValidationResult Validate(IValidatedObject validated, IValidationContext validationContext);
}