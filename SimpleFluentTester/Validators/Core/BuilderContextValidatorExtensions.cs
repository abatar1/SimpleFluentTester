using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleFluentTester.Validators.Core;

internal static class BuilderContextValidatorExtensions
{
    public static bool Validate(this IValidatedObject validated)
    {
        var results = validated.GetValidationResults();
        return results.All(x => x.Status == ValidationStatus.Valid);
    }
    
    public static IValidatedObject AddReadyValidation(
        this IValidatedObject validated,
        ValidationResult validationResult)
    {
        return AddValidation(validated, validationResult.Subject, () => validationResult);
    }

    public static IValidatedObject RegisterFutureValidation<TValidator>(
        this IValidatedObject validated,
        Func<IValidationContext>? validationContextFactory = null)
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

        AddValidation(validated, validator.Subject, () => RunValidation(validator, validated, validationContextFactory));
        return validated;
    }

    private static ValidationResult RunValidation(
        IValidator validator, 
        IValidatedObject validated, 
        Func<IValidationContext>? validationContextFactory = null)
    {
        try
        {
            var validationContext = validationContextFactory?.Invoke() ?? new EmptyValidationContext();
            return validator.Validate(validated, validationContext);
        }
        catch (Exception e)
        {
            var message = $"Failed to validate {validated.GetType()} with {validator.GetType()} validator.";
            return ValidationResult.Failed(validator.Subject, e, message);
        }
    }
    
    private static IValidatedObject AddValidation(
        IValidatedObject validated,
        ValidationSubject subject,
        Func<ValidationResult> validationFactory)
    {
        if (validated.Validations.ContainsKey(subject))
            validated.Validations[subject].Add(new Lazy<ValidationResult>(validationFactory));
        else
            validated.Validations[subject] = new List<Lazy<ValidationResult>> { new(validationFactory) };
        return validated;
    }
}