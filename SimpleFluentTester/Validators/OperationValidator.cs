using System;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class OperationValidator : BaseValidator<OperationValidationContext, TestCase>
{
    public override Type AllowedType => ValidatedTypes.TestCase;
    
    public override ValidationSubject Subject => ValidationSubject.Operation;

    protected override ValidationResult ValidateCore(
        TestCase testCase, 
        OperationValidationContext validationContext)
    {
        if ( validationContext.Operation == null)
            return NonValid("Operation not specified");
        
        var returnParameterType =  validationContext.Operation.Method.ReturnParameter?.ParameterType;
        
        if (returnParameterType == typeof(void) || returnParameterType == null)
            return NonValid("Operation must have return type to be testable");
        
        if (testCase.Expected.Variety == ComparedObjectVariety.Exception)
            return Ok();
        
        var returnUnderlyingType = Nullable.GetUnderlyingType(returnParameterType);
        var isNullable = false;
        if (returnUnderlyingType != null)
        {
            returnParameterType = returnUnderlyingType;
            isNullable = true;
        }

        if (isNullable && testCase.Expected.Variety == ComparedObjectVariety.Null)
            return Ok();
        
        if (returnParameterType != testCase.Expected.Type)
            return ValidationResult.NonValid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
        
        return Ok();
    }
}

public sealed class OperationValidationContext(Delegate? operation) : IValidationContext
{
    public Delegate? Operation { get; } = operation;
}