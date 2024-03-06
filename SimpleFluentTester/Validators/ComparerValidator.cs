using System;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class ComparerValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(
        ITestSuiteBuilderContext<TOutput> context, 
        IValidatedObject validatedObject)
    {
        if (context.Comparer != null)
            return ValidationResult.Ok(ValidationSubject.Comparer);
        
        if (!typeof(IEquatable<TOutput>).IsAssignableFrom(typeof(TOutput)))
            return ValidationResult.Failed(ValidationSubject.Comparer, $"{nameof(TOutput)} type should be assignable from {nameof(IEquatable<TOutput>)} or comparer should be defined");
           
        return ValidationResult.Ok(ValidationSubject.Comparer);
    }
}