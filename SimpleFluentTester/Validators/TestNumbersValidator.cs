using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class TestNumbersValidator : BaseValidator<TestNumbersValidatedObject>
{
    public override ISet<Type> AllowedTypes => new HashSet<Type>([ValidatedTypes.Context]);
    public override ValidationSubject Subject => ValidationSubject.TestNumbers;
    
    public override ValidationResult Validate(
        IValidated validated, 
        IValidatedObject validatedObject)
    {
        var testCasesValidatedObject = CastValidatedObject(validatedObject);
        var context = CastValidated<ITestSuiteContext>(validated);
        
        var testNumbersHash = testCasesValidatedObject.TestNumbers;
        if (testNumbersHash.Count != 0 && (testNumbersHash.Count > context.TestCases.Count || testNumbersHash.Max() > context.TestCases.Count))
            return NonValid("Invalid test case numbers were given as input");
        
        return Ok();
    }
}

public sealed class TestNumbersValidatedObject(ISet<int> testNumbers) : IValidatedObject
{
    public ISet<int> TestNumbers { get; } = testNumbers;
}