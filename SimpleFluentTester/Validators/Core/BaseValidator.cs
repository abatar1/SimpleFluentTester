using System;
using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

internal abstract class BaseValidator<TValidatedObject> : IValidator
    where TValidatedObject : IValidatedObject
{
    protected BaseValidator()
    {
        Key = GetType().Name;
    }

    public virtual string Key { get; }

    public abstract ISet<Type> AllowedTypes { get; }
    
    public abstract ValidationSubject Subject { get; }

    public abstract ValidationResult Validate(IValidated validated, IValidatedObject validatedObject);

    protected ValidationResult Ok()
    {
        return ValidationResult.Valid(Subject);
    }
    
    protected ValidationResult NonValid(string message)
    {
        return ValidationResult.NonValid(Subject, message);
    }
    
    protected TValidated CastValidated<TValidated>(IValidated validated)
        where TValidated : IValidated
    {
        if (validated is not TValidated castedValidated)
            throw new ValidationUnexpectedException("Was not able to cast validated to it's type, seems like a bug.");
        return castedValidated;
    }
    
    protected TValidatedObject CastValidatedObject(IValidatedObject validated)
    {
        if (validated is not TValidatedObject castedValidated)
            throw new ValidationUnexpectedException("Was not able to cast validated object to it's type, seems like a bug.");
        return castedValidated;
    }
}