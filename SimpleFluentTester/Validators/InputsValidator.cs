using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class InputsValidator : BaseValidator<EmptyValidationContext, DefinedTestCase>
{
    public override ValidationSubject Subject => ValidationSubject.Inputs;

    protected override SubjectValidation ValidateCore(
        DefinedTestCase testCase, 
        EmptyValidationContext _)
    {
        var inputs = testCase.Inputs;
        var operationParameterInfos = testCase.OperationFactory.Value?.Method.GetParameters().ToList();

        if (inputs.Count != operationParameterInfos?.Count)
        {
            var formattedInputs = string.Join(", ", inputs.Select(x => x.ToString()));
            return new SubjectValidation(new List<ValidationResult> { NonValid($"Invalid inputs number, should have {operationParameterInfos?.Count} parameters, but had {inputs.Count}: [{formattedInputs}]") });
        }

        var parametersTypesAreValid = inputs
            .Zip(operationParameterInfos, (input, parameter) => (input, parameter))
            .All(x => ValidateInputType(x.input, x.parameter.ParameterType));
        if (!parametersTypesAreValid)
            return new SubjectValidation(new List<ValidationResult> { NonValid("Passed parameters and expected operation parameters are not equal.") });

        var validationResults = new List<ValidationResult>();
        foreach (var clause in testCase.Clauses)
        {
            validationResults.Add(ValidateInternalSingle(clause, operationParameterInfos));
        }
        return new SubjectValidation(validationResults);
    }
    
    private ValidationResult ValidateInternalSingle(ITestClause testClause, List<ParameterInfo> operationParameterInfos)
    {
        if (testClause.Expected.Variety == ComparedObjectVariety.Parameter)
        {
            var parameterInfo = ObjectParameterExtractor.ExtractExpectedParameter(testClause);
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
