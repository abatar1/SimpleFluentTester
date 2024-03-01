using System;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

public sealed class ComparerValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(TestRunBuilderContext<TOutput> context, IValidatedObject validatedObject)
    {
        if (context.Comparer != null || context.IsObjectOutput)
            return ValidationResult.Ok(ValidationSubject.Comparer);
        
        if (!typeof(IEquatable<TOutput>).IsAssignableFrom(typeof(TOutput)))
            return ValidationResult.Failed(ValidationSubject.Comparer, "TOutput type should be assignable from IEquatable<TOutput> or comparer should be defined");
           
        return ValidationResult.Ok(ValidationSubject.Comparer);
    }
}

