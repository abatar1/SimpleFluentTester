using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleFluentTester.Validators.Core;

public static class ValidationPipe
{
    /// <summary>
    /// Invokes all validations for the object inherited from <see cref="IValidatedObject"/> interface.
    /// All validation's results of a single object packed into single object.
    /// </summary>
    public static PackedValidation ValidatePacked(IValidatedObject validated)
    {
        var generalStatus = ValidationStatus.Valid;
        var validationResults = validated.Validations
            .Select(x =>
            {
                var validationResults = x.Value
                    .Select(y => y.Invoke())
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
        return new PackedValidation(validated, validationResults, generalStatus);
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