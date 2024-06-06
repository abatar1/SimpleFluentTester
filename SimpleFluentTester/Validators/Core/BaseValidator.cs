using System;

namespace SimpleFluentTester.Validators.Core;

internal abstract class BaseValidator<TValidationContext, TValidatedObject> : IValidator
    where TValidationContext : IValidationContext
    where TValidatedObject : IValidatedObject
{
    protected BaseValidator()
    {
        Key = GetType().Name;
    }

    public virtual string Key { get; }

    public abstract Type AllowedType { get; }
    
    public abstract ValidationSubject Subject { get; }

    public ValidationResult Validate(IValidatedObject validated, IValidationContext validationContext)
    {
        var castedContext = CastValidationContext(validationContext);
        var castedValidated = CastValidatedObject(validated);
        return ValidateCore(castedValidated, castedContext);
    }

    protected abstract ValidationResult ValidateCore(TValidatedObject validated, TValidationContext validationContext);

    protected ValidationResult Ok()
    {
        return ValidationResult.Valid(Subject);
    }
    
    protected ValidationResult NonValid(string message)
    {
        return ValidationResult.NonValid(Subject, message);
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