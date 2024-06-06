using System;
using System.Linq;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class InputsValidator : BaseValidator<InputsValidationContext, TestCase>
{
    public override Type AllowedType => ValidatedTypes.TestCase;
    
    public override ValidationSubject Subject => ValidationSubject.Inputs;

    protected override ValidationResult ValidateCore(
        TestCase testCase, 
        InputsValidationContext validationContext)
    {
        var inputs = testCase.Inputs;
        var operationParameterInfos = validationContext.Operation?.Method.GetParameters().ToList();

        if (inputs.Length != operationParameterInfos?.Count)
        {
            var formattedInputs = string.Join(", ", inputs.Select(x => x.ToString()));
            return NonValid($"Invalid inputs number, should be {operationParameterInfos?.Count}, but was {inputs.Length}.");
        }

        var parametersTypesAreValid = inputs
            .Zip(operationParameterInfos, (input, parameter) => (input, parameter))
            .All(x => ValidateInputType(x.input, x.parameter.ParameterType));
        if (!parametersTypesAreValid)
            return NonValid("Passed parameters and expected operation parameters are not equal.");
        
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

public sealed class InputsValidationContext(Delegate? operation)
    : IValidationContext
{
    public Delegate? Operation { get; } = operation;
}