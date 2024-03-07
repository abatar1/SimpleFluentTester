using System;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class OperationValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(
        ITestSuiteBuilderContext<TOutput> context, 
        IValidatedObject validatedObject)
    {
        if (validatedObject is not OperationValidatedObject nonTypedOperationValidatedObject)
            throw new ValidationUnexpectedException("Was not able to cast validated object to it's type, seems like a bug.");

        if (context.Operation == null)
            return ValidationResult.Failed(ValidationSubject.Operation, "Operation not specified");
        
        if (nonTypedOperationValidatedObject.ActualOperationType == typeof(object))
            return ValidationResult.Ok(ValidationSubject.Operation);
        
        if (context.Operation.Method.ReturnParameter!.ParameterType == typeof(void))
            return ValidationResult.Failed(ValidationSubject.Operation, "Operation must have return type to be testable");

        Type? returnParameterType;
        var actualOperationType = nonTypedOperationValidatedObject.ActualOperationType;

        if (context.OutputUnderlyingType != null)
        {
            if (actualOperationType == null)
                return ValidationResult.Ok(ValidationSubject.Operation);
            returnParameterType = context.OutputUnderlyingType;
        }
        else
        {
            returnParameterType = context.Operation.Method.ReturnParameter.ParameterType;
        }

        if (returnParameterType != actualOperationType)
            return ValidationResult.Failed(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
        
        return ValidationResult.Ok(ValidationSubject.Operation);
    }
}

public sealed class OperationValidatedObject(Type? actualOperationType) : IValidatedObject
{
    public Type? ActualOperationType { get; } = actualOperationType;
}