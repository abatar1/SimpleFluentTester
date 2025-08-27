using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Helpers.TestObjects;

internal sealed class OkTestValidator(ValidationSubject validationSubject) 
    : BaseValidator<EmptyValidationContext, DefinedTestCase>
{
    public override ValidationSubject Subject => validationSubject;

    protected override SubjectValidation ValidateCore(DefinedTestCase validatedObject, EmptyValidationContext validationContext)
    {
        return Ok();
    }
}