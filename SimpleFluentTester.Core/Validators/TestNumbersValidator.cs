using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

public sealed class TestNumbersValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(TestRunBuilderContext<TOutput> context, IValidatedObject validatedObject)
    {
        if (validatedObject is not TestNumbersValidatedObject testCasesValidatedObject)
            throw new ValidationUnexpectedException("Was not able to cast validated object to it's type, seems like a bug.");
        
        var testNumbersHash = testCasesValidatedObject.TestNumbers;
        if (testNumbersHash.Count != 0 && (testNumbersHash.Count > context.TestCases.Count || testNumbersHash.Max() > context.TestCases.Count))
            return ValidationResult.Failed(ValidationSubject.TestNumbers, "Invalid test case numbers were given as input");
        
        return ValidationResult.Ok(ValidationSubject.TestNumbers);
    }
}

public sealed class TestNumbersValidatedObject(ISet<int> testNumbers) : IValidatedObject
{
    public ISet<int> TestNumbers { get; } = testNumbers;
}