using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

public sealed class InputsValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(TestSuiteBuilderContext<TOutput> context, IValidatedObject validatedObject)
    {
        if (validatedObject is not InputsValidatedObject inputsValidatedObject)
            throw new ValidationUnexpectedException("Was not able to cast validated object to it's type, seems like a bug.");

        var inputs = inputsValidatedObject.Inputs;
        var operationParameterInfos = context.OperationParameters;
        
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