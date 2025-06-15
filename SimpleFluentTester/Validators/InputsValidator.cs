using System;
using System.Linq;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class InputsValidator : BaseValidator<EmptyValidationContext, DeferredTestCase>
{
    public override ValidationSubject Subject => ValidationSubject.Inputs;

    protected override ValidationResult ValidateCore(
        DeferredTestCase testCase, 
        EmptyValidationContext _)
    {
        var inputs = testCase.Inputs;
        var operationParameterInfos = testCase.OperationFactory.Value?.Method.GetParameters().ToList();

        if (inputs.Length != operationParameterInfos?.Count)
        {
            var formattedInputs = string.Join(", ", inputs.Select(x => x.ToString()));
            return NonValid($"Invalid inputs number, should be {operationParameterInfos?.Count}, but was {formattedInputs}.");
        }

        var parametersTypesAreValid = inputs
            .Zip(operationParameterInfos, (input, parameter) => (input, parameter))
            .All(x => ValidateInputType(x.input, x.parameter.ParameterType));
        if (!parametersTypesAreValid)
            return NonValid("Passed parameters and expected operation parameters are not equal.");
        
        if (testCase.Expected.Variety == ComparedObjectVariety.Parameter)
        {
            var parameterInfo = ObjectParameterExtractor.ExtractExpectedParameter(testCase);
            var hasParameter = operationParameterInfos.Any(inputParameterInfo => inputParameterInfo.MetadataToken == parameterInfo.MetadataToken);
            if (!hasParameter)
                return NonValid($"Could not find parameter with name {parameterInfo.Name} and position {parameterInfo.Position}.");
        }
        
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
