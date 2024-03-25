using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class OperationValidator : BaseValidator<OperationValidatedObject>
{
    public override ISet<Type> AllowedTypes => new HashSet<Type>([ValidatedTypes.TestCase]);
    public override ValidationSubject Subject => ValidationSubject.Operation;

    public override ValidationResult Validate(
        IValidated validated, 
        IValidatedObject validatedObject)
    {
        var operation = CastValidatedObject(validatedObject).Operation;
        
        if (operation == null)
            return NonValid("Operation not specified");
        
        var returnParameterType = operation.Method.ReturnParameter!.ParameterType;
        
        if (returnParameterType == typeof(void))
            return NonValid("Operation must have return type to be testable");
        
        var testCase = CastValidated<TestCase>(validated);
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

public sealed class OperationValidatedObject(Delegate? operation) : IValidatedObject
{
    public Delegate? Operation { get; } = operation;
}