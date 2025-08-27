using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.Validators.Helpers;

public static class ValidatedObjectExtensions
{
    public static bool IsValid(this IValidatedObject validatedObject)
    {
        return validatedObject.Validations.Values
            .SelectMany(x => x)
            .All(x => IsValid(x.Value));
    }
    
    public static bool IsValid(this SubjectValidation subjectValidation)
    {
        return subjectValidation.Validations
            .All(x => x.Status == ValidationStatus.Valid);
    }

    public static IReadOnlyCollection<ValidationResult> GetNonValidValidations(this IValidatedObject validatedObject)
    {
        return validatedObject.Validations.Values
            .SelectMany(x => x)
            .Select(x => GetNonValidValidations(x.Value))
            .SelectMany(x => x)
            .ToList();
    }
    
    public static IReadOnlyCollection<ValidationResult> GetNonValidValidations(this SubjectValidation subjectValidation)
    {
        return subjectValidation.Validations
            .Where(x => x.Status != ValidationStatus.Valid)
            .ToList();
    }
    
    public static IReadOnlyCollection<ValidationResult> GetValidations(this IValidatedObject validatedObject)
    {
        return validatedObject.Validations.Values.SelectMany(x => x)
            .SelectMany(x => x.Value.Validations)
            .ToList();
    }
}