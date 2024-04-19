using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
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
        {
            var formattedInputs = string.Join(", ", inputs.Select(x => x.ToString()));
            return NonValid($"Invalid inputs number, should be {operationParameterInfos?.Count}, but was {inputs.Length}, inputs {formattedInputs}");
        }

        var parametersTypesAreValid = inputs
            .Zip(operationParameterInfos, (input, parameter) => (input, parameter))
            .All(x => ValidateInputType(x.input, x.parameter.ParameterType));
        if (!parametersTypesAreValid)
            return NonValid("Passed parameters and expected operation parameters are not equal");
        
        return Ok();
    }
    
    private static bool ValidateInputType(IComparedObject input, Type parameterType)
    {
        var underlyingReturnParameterType = Nullable.GetUnderlyingType(parameterType);
        if (underlyingReturnParameterType == null) 
            return input.Variety != ComparedObjectVariety.Null && input.Type == parameterType;
        
        if (input.Variety == ComparedObjectVariety.Null)
            return true;

        return input.Type == underlyingReturnParameterType;
    }
}

public sealed class InputsValidatedObject(Delegate? operation)
    : IValidatedObject
{
    public Delegate? Operation { get; } = operation;
}