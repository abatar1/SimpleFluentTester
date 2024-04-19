using System;
using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

internal static class BuilderContextValidatorExtensions
{
    public static IValidated AddValidation(
        this IValidated validated,
        ValidationResult validationResult)
    {
        return AddValidation(validated, validationResult.Subject, () => validationResult);
    }

    public static IValidated RegisterValidation<TValidator>(
        this IValidated validated,
        Func<IValidatedObject>? validatedObjectFactory = null)
        where TValidator : IValidator
    {
        IValidator validator;
        try
        {
            validator = (IValidator)Activator.CreateInstance(typeof(TValidator));
        }
        catch (Exception e)
        {
            var message = $"Couldn't register validator for a type {typeof(TValidator)}.";
            throw new InvalidOperationException(message, e);
        }

        var validatedType = validated.GetType();
        if (!validator.AllowedTypes.Contains(validatedType))
        {
            var message =
                $"Could not register validator {validator.GetType()} for this {nameof(IValidated)} type {validatedType}, it is not allowed";
            throw new InvalidOperationException(message);
        }

        AddValidation(validated, validator.Subject, () => RunValidation(validator, validated, validatedObjectFactory));
        return validated;
    }

    private static ValidationResult RunValidation(
        IValidator validator, 
        IValidated validated, 
        Func<IValidatedObject>? validatedObjectFactory)
    {
        try
        {
            var validatedObject = validatedObjectFactory?.Invoke() ?? new EmptyValidatedObject();
            return validator.Validate(validated, validatedObject);
        }
        catch (Exception e)
        {
            var message = $"Failed to validate {validated.GetType()} with {validator.GetType()} validator.";
            return ValidationResult.Failed(validator.Subject, e, message);
        }
    }
    
    private static IValidated AddValidation(
        IValidated validated,
        ValidationSubject subject,
        Func<ValidationResult> validationFactory)
    {
        if (validated.Validations.ContainsKey(subject))
            validated.Validations[subject].Add(validationFactory);
        else
            validated.Validations[subject] = new List<Func<ValidationResult>> { validationFactory };
        return validated;
    }
}