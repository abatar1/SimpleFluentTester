using System;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

public sealed class OperationValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(TestSuiteBuilderContext<TOutput> context, IValidatedObject validatedObject)
    {
        if (validatedObject is not OperationValidatedObject nonTypedOperationValidatedObject)
            throw new ValidationUnexpectedException("Was not able to cast validated object to it's type, seems like a bug.");

        if (context.Operation.Value == null)
            return ValidationResult.Failed(ValidationSubject.Operation, "Operation haven't been specified, please use UseOperation method or TestSuiteDelegateAttribute");

        if (context.Operation.Value.Method.ReturnParameter == null)
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
            returnParameterType = context.Operation.Value.Method.ReturnParameter.ParameterType;
        }

        if (returnParameterType != actualOperationType)
            return ValidationResult.Failed(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
        
        return ValidationResult.Ok(ValidationSubject.Operation);
    }
}

public sealed class OperationValidatedObject(
    Type? actualOperationType)
    : IValidatedObject
{
    public Type? ActualOperationType { get; } = actualOperationType;
}