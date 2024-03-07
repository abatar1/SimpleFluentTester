using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class InputsValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(
        ITestSuiteBuilderContext<TOutput> context, 
        IValidatedObject validatedObject)
    {
        if (validatedObject is not InputsValidatedObject inputsValidatedObject)
            throw new ValidationUnexpectedException("Was not able to cast validated object to it's type, seems like a bug.");

        var inputs = inputsValidatedObject.Inputs;
        var operationParameterInfos = context.Operation?.Method.GetParameters().ToList();

        if (operationParameterInfos == null)
            return ValidationResult.Failed(ValidationSubject.Inputs, "Operation hasn't been specified before validation, seems like a bug.");
        
        if (inputs.Count != operationParameterInfos.Count)
            return ValidationResult.Failed(ValidationSubject.Inputs, $"Invalid inputs number, should be {operationParameterInfos.Count}, but was {inputs.Count}, inputs {string.Join(", ", inputs)}");

        var parametersTypesAreValid = inputs
            .Zip(operationParameterInfos, (input, parameter) => (input, parameter))
            .All(x => ValidateInputType(x.input, x.parameter.ParameterType));
        if (!parametersTypesAreValid)
            return ValidationResult.Failed(ValidationSubject.Inputs, "Passed parameters and expected operation parameters are not equal");
        
        return ValidationResult.Ok(ValidationSubject.Inputs);
    }
    
    private static bool ValidateInputType(object? input, Type parameterType)
    {
        var underlyingReturnParameterType = Nullable.GetUnderlyingType(parameterType);
        if (underlyingReturnParameterType == null) 
            return input != null && input.GetType() == parameterType;
        
        if (input == null)
            return true;

        return input.GetType() == underlyingReturnParameterType;
    }
}

public sealed class InputsValidatedObject(IReadOnlyCollection<object?> inputs)
    : IValidatedObject
{
    public IReadOnlyCollection<object?> Inputs { get; } = inputs;
}