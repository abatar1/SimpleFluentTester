using System;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class ComparerValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(
        ITestSuiteBuilderContext<TOutput> context, 
        IValidatedObject validatedObject)
    {
        Type? comparerType;
        if (context.Comparer == null)
        {
            if (typeof(TOutput) == typeof(object))
                comparerType = context.Operation?.Method.ReturnParameter?.ParameterType;
            else
                comparerType = typeof(TOutput);
        }
        else
            comparerType = context.Comparer.Method.ReturnParameter?.ParameterType;

        var interfaceType = typeof(IEquatable<>).MakeGenericType(comparerType);
        if (!interfaceType.IsAssignableFrom(comparerType))
            return ValidationResult.Failed(ValidationSubject.Comparer, $"{nameof(TOutput)} type should be assignable from {nameof(IEquatable<TOutput>)} or comparer should be defined");
           
        return ValidationResult.Ok(ValidationSubject.Comparer);
    }
}