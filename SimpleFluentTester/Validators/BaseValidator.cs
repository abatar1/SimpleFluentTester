using System.Collections.Generic;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.Validators;

internal abstract class BaseValidator<TValidationContext, TValidatedObject> : IValidator
    where TValidationContext : IValidationContext
    where TValidatedObject : IValidatedObject
{
    protected BaseValidator()
    {
        Key = GetType().Name;
    }

    public virtual string Key { get; }
    
    public abstract ValidationSubject Subject { get; }

    public SubjectValidation Validate(IValidatedObject validated, IValidationContext validationContext)
    {
        var castedContext = CastValidationContext(validationContext);
        var castedValidated = CastValidatedObject(validated);
        return ValidateCore(castedValidated, castedContext);
    }

    protected abstract SubjectValidation ValidateCore(TValidatedObject validated, TValidationContext validationContext);

    protected SubjectValidation Ok()
    {
        return new SubjectValidation(new List<ValidationResult> { ValidationResult.Valid(Subject) });
    }
    
    protected SubjectValidation NonValid(string message)
    {
        return new SubjectValidation(new List<ValidationResult> { ValidationResult.NonValid(Subject, message) });
    }
    
    private static TValidatedObject CastValidatedObject(IValidatedObject validated)
    {
        if (validated is not TValidatedObject castedValidated)
            throw new ValidationUnexpectedException("Was not able to cast validated object to it's type, seems like a bug.");
        return castedValidated;
    }
    
    private static TValidationContext CastValidationContext(IValidationContext validated)
    {
        if (validated is not TValidationContext castedValidationContext)
            throw new ValidationUnexpectedException("Was not able to cast validation context to it's type, seems like a bug.");
        return castedValidationContext;
    }
}