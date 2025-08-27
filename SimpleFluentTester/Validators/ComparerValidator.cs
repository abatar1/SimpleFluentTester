using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.Validators;

internal sealed class ComparerValidator : BaseValidator<EmptyValidationContext, DefinedTestCase>
{
    public override ValidationSubject Subject => ValidationSubject.Comparer;

    protected override SubjectValidation ValidateCore(
        DefinedTestCase testCase, 
        EmptyValidationContext _)
    {
        var validationResults = new List<SubjectValidation>();
        foreach (var clause in testCase.Clauses)
        {
            validationResults.Add(ValidateInternalSingle(testCase, clause));
        }
        return new SubjectValidation(validationResults);
    }
    
    private SubjectValidation ValidateInternalSingle(DefinedTestCase testCase, ITestClause testClause)
    {
        var testCaseExpectedObjectType = GetExpectedObjectType(testClause);

        if (testCaseExpectedObjectType == null)
            return Ok();
        
        var comparer = testCase.ComparerFactory.Value;

        if (comparer == null)
            return ValidateWithoutComparer(testCaseExpectedObjectType);
        
        if (comparer.Method.ReturnParameter?.ParameterType != typeof(bool))
            return NonValid($"Return type of the custom comparer is not bool but {comparer.Method.ReturnParameter?.ParameterType}, something went wrong during initialization");

        var parameters = comparer.Method.GetParameters();
        if (parameters.Length != 2)
            return NonValid($"Custom comparer has {parameters.Length} parameters, but should has 2, something went wrong during initialization");

        var expectParamType = parameters[0].ParameterType;
        var resultParamType = parameters[1].ParameterType;

        if (expectParamType != resultParamType)
            return NonValid("Comparer has not the same input parameter type, something went wrong during initialization");

        Type? elementType = testCaseExpectedObjectType;
        if (CollectionHelper.IsCollection(elementType))
            elementType = CollectionHelper.GetCollectionElementType(elementType);
        
        Type? expectedType = expectParamType;
        if (CollectionHelper.IsCollection(expectedType))
            expectedType = CollectionHelper.GetCollectionElementType(expectedType);
        
        if (expectedType != elementType)
            return NonValid($"Test case type was {testCaseExpectedObjectType}, but comparer type is {expectParamType}");
           
        return Ok();
    }

    private SubjectValidation ValidateWithoutComparer(Type testCaseExpectedObjectType)
    {
        if (CollectionHelper.IsCollection(testCaseExpectedObjectType))
        {
            Type? elementType = CollectionHelper.GetCollectionElementType(testCaseExpectedObjectType);
            if (elementType == null)
                return NonValid($"Cannot determine the element type of {testCaseExpectedObjectType}");

            var equatableElementType = typeof(IEquatable<>).MakeGenericType(elementType);
            if (!equatableElementType.IsAssignableFrom(elementType))
            {
                return NonValid($"{elementType} elements should implement {typeof(IEquatable<>).Name} or a comparer should be defined");
            }
        }
        else
        {
            var interfaceType = typeof(IEquatable<>).MakeGenericType(testCaseExpectedObjectType);
            if (!interfaceType.IsAssignableFrom(testCaseExpectedObjectType))
                return NonValid($"{testCaseExpectedObjectType} type should be assignable from {typeof(IEquatable<>).Name} or comparer should be defined");
        }

        return Ok();
    }

    private Type? GetExpectedObjectType(ITestClause testClause)
    {
        return testClause.Expected.Variety switch
        {
            ComparedObjectVariety.Null => null,
            ComparedObjectVariety.Exception => null,
            ComparedObjectVariety.Value => testClause.Expected.Type,
            ComparedObjectVariety.Parameter => ObjectParameterExtractor.ExtractExpectedParameterType(testClause),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}