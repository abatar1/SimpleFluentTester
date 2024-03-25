using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class InputsValidator : BaseValidator<InputsValidatedObject>
{
    public override ISet<Type> AllowedTypes => new HashSet<Type>([ValidatedTypes.TestCase]);
    public override ValidationSubject Subject => ValidationSubject.Inputs;

    public override ValidationResult Validate(
        IValidated validated, 
        IValidatedObject validatedObject)
    {
        var inputsValidatedObject = CastValidatedObject(validatedObject);
        var testCase = CastValidated<TestCase>(validated);
        
        var inputs = testCase.Inputs;
        var operationParameterInfos = inputsValidatedObject.Operation?.Method.GetParameters().ToList();
        
        if (inputs.Length != operationParameterInfos?.Count)
            return NonValid($"Invalid inputs number, should be {operationParameterInfos?.Count}, but was {inputs.Length}, inputs {string.Join(", ", inputs)}");

        var parametersTypesAreValid = inputs
            .Zip(operationParameterInfos, (input, parameter) => (input, parameter))
            .All(x => ValidateInputType(x.input, x.parameter.ParameterType));
        if (!parametersTypesAreValid)
            return NonValid("Passed parameters and expected operation parameters are not equal");
        
        return Ok();
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

public sealed class InputsValidatedObject(Delegate? operation)
    : IValidatedObject
{
    public Delegate? Operation { get; } = operation;
}