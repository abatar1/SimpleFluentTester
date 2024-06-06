using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class TestNumbersValidator : BaseValidator<TestNumbersValidationContext, ITestSuiteContext>
{
    public override Type AllowedType => ValidatedTypes.Context;
    
    public override ValidationSubject Subject => ValidationSubject.TestNumbers;
    
    protected override ValidationResult ValidateCore(
        ITestSuiteContext testSuiteContext, 
        TestNumbersValidationContext validationContext)
    {
        var testNumbersHash = validationContext.TestNumbers;
        if (testNumbersHash.Count != 0 && (testNumbersHash.Count > testSuiteContext.TestCases.Count || testNumbersHash.Max() > testSuiteContext.TestCases.Count))
            return NonValid("Invalid test case numbers were given as input");
        
        return Ok();
    }
}

public sealed class TestNumbersValidationContext(ISet<int> testNumbers) : IValidationContext
{
    public ISet<int> TestNumbers { get; } = testNumbers;
}