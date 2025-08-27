using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.Validators.Helpers;

internal static class BuilderContextValidatorExtensions
{
    /// <summary>
    /// Invokes all validations for the object inherited from <see cref="IValidatedObject"/> interface.
    /// All validation's results of a single object are packed into a single object.
    /// </summary>
    public static ValidationStatus Validate(this IValidatedObject validated)
    {
        var generalStatus = ValidationStatus.Valid;
        _ = validated.Validations
            .Select(x =>
            {
                var validationResults = x.Value
                    .SelectMany(y => y.Value.Validations)
                    .ToList();

                var currentValidationStatus = ValidationStatus.Valid;
                var notValid = validationResults
                    .Any(y => y.Status == ValidationStatus.NonValid);
                if (notValid)
                {
                    currentValidationStatus = ValidationStatus.NonValid;
                    if (generalStatus == ValidationStatus.Valid)
                        generalStatus = ValidationStatus.NonValid;
                }

                var failed = validationResults
                    .Any(y => y.Status == ValidationStatus.Failed);
                if (failed)
                {
                    currentValidationStatus = ValidationStatus.Failed;
                    generalStatus = ValidationStatus.Failed;
                }

                var message = GetAggregatedMessage(validationResults);
                var aggregateException = GetAggregateException(validationResults);
                return ValidationResult.FromStatus(currentValidationStatus, x.Key, message, aggregateException);
            })
            .ToList();
        return generalStatus;
    }

    /// <summary>
    /// Adds an already computed validation result to the specified validated object.
    /// </summary>
    /// <param name="validated">The object to which the validation result is added, implementing <see cref="IValidatedObject"/>.</param>
    /// <param name="validationResult">The validation results to be associated with the validated object.</param>
    /// <returns>The validated object with the newly added validation result.</returns>
    public static IValidatedObject AddReadyValidation(
        this IValidatedObject validated,
        ValidationResult validationResult)
    {
        return AddValidation(validated, validationResult.Subject, () => new SubjectValidation(new List<ValidationResult> { validationResult }));
    }

    /// <summary>
    /// Registers a future validation on the given validated object using the specified validator type.
    /// The future validation is created using an instance of <typeparamref name="TValidator"/> and added to the validated object.
    /// </summary>
    /// <typeparam name="TValidator">The type of the validator to be used for the future validation, which must implement <see cref="IValidator"/>.</typeparam>
    /// <param name="validated">The validated object to which the future validation is added.</param>
    /// <param name="args">Optional arguments for creating the validator instance, if required.</param>
    /// <param name="validationContextFactory">Optional factory function that creates validation context objects for the validation process.</param>
    /// <returns>The same validated object passed as input with the future validation registered.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the validator instance could not be created due to an error.</exception>
    public static IValidatedObject RegisterFutureValidation<TValidator>(
        this IValidatedObject validated,
        object[]? args = null,
        Func<IValidationContext>? validationContextFactory = null)
        where TValidator : IValidator
    {
        IValidator validator;
        try
        {
            validator = (IValidator)Activator.CreateInstance(typeof(TValidator), args);
        }
        catch (Exception e)
        {
            var message = $"Couldn't register validator for a type {typeof(TValidator)}.";
            throw new InvalidOperationException(message, e);
        }

        AddValidation(validated, validator.Subject, () => RunValidation(validator, validated, validationContextFactory));
        return validated;
    }

    private static SubjectValidation RunValidation(
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
            return new SubjectValidation(new List<ValidationResult> { ValidationResult.Failed(validator.Subject, e, message) });
        }
    }
    
    private static IValidatedObject AddValidation(
        IValidatedObject validated,
        ValidationSubject subject,
        Func<SubjectValidation> validationFactory)
    {
        if (validated.Validations.ContainsKey(subject))
            validated.Validations[subject].Add(new Lazy<SubjectValidation>(validationFactory));
        else
            validated.Validations[subject] = new List<Lazy<SubjectValidation>> { new(validationFactory) };
        return validated;
    }
    
    
    private static string? GetAggregatedMessage(IList<ValidationResult> validationResults)
    {
        var messages = validationResults
            .Where(y => !string.IsNullOrWhiteSpace(y.Message))
            .Select(y => y.Message)
            .ToList();
        string? message = null;
        if (messages.Count != 0)
            message = string.Join(Environment.NewLine, messages);
        return message;
    }

    private static AggregateException? GetAggregateException(IList<ValidationResult> validationResults)
    {
        var exceptions = validationResults
            .Where(y => y.Exception != null)
            .Select(y => y.Exception)
            .ToList();
        AggregateException? aggregateException = null;
        if (exceptions.Count != 0)
            aggregateException = new AggregateException(exceptions);
        return aggregateException;
    }
}