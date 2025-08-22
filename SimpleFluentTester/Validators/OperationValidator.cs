using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class OperationValidator : BaseValidator<EmptyValidationContext, DefinedTestCase>
{
    public override ValidationSubject Subject => ValidationSubject.Operation;

    protected override SubjectValidation ValidateCore(
        DefinedTestCase testCase, 
        EmptyValidationContext _)
    {
        var operation = testCase.OperationFactory.Value;

        if (operation == null)
            return new SubjectValidation(new List<ValidationResult> { NonValid("Operation not specified") });
        
        var returnParameterType = operation.Method.ReturnParameter?.ParameterType;
        
        var validationResults = new List<ValidationResult>();
        foreach (var clause in testCase.Clauses)
        {
            validationResults.Add(ValidateInternalSingle(clause, returnParameterType));
        }
        return new SubjectValidation(validationResults);
    }

    private ValidationResult ValidateInternalSingle(ITestClause testClause, Type? returnParameterType)
    {
        switch (testClause.Expected.Variety)
        {
            case ComparedObjectVariety.Null:
                if (CheckIfReturnIsVoid(returnParameterType))
                    return NonValid("Operation must have return type to be testable");
                var (isNullable, underlyingReturnType) = GetUnderlyingReturnType(returnParameterType);
                if (isNullable || underlyingReturnType == testClause.Expected.Type)
                    return Ok();
                return ValidationResult.NonValid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
            
            case ComparedObjectVariety.Value:
                if (CheckIfReturnIsVoid(returnParameterType))
                    return NonValid("Operation must have return type to be testable");
                (var _, underlyingReturnType) = GetUnderlyingReturnType(returnParameterType);
                if (underlyingReturnType == testClause.Expected.Type)
                    return Ok();
                return ValidationResult.NonValid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
            
            case ComparedObjectVariety.Exception:
                // Operation type is always ok for exceptions, we can't validate an expected result here.
                return Ok();
            
            case ComparedObjectVariety.Parameter:
                // Parameter objects should be validated via input parameters, so here we just skip them.
                return Ok();
            
            default:
                throw new ArgumentOutOfRangeException(nameof(testClause.Expected.Variety));
        }
    }
    
    private static bool CheckIfReturnIsVoid([NotNullWhen(false)]Type? type) => type == typeof(void) || type == null;
    
    private static (bool, Type) GetUnderlyingReturnType(Type type)
    {
        var returnUnderlyingType = Nullable.GetUnderlyingType(type);
        if (returnUnderlyingType != null)
            return (true, returnUnderlyingType);
        return (false, type);
    }
}
