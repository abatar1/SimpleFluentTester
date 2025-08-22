using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Helpers.TestObjects;

internal sealed class CustomValidator(ValidationSubject validationSubject) 
    : BaseValidator<EmptyValidationContext, EmptyValidatedObject>
{
    public override ValidationSubject Subject => validationSubject;

    protected override SubjectValidation ValidateCore(EmptyValidatedObject validatedObject, EmptyValidationContext validationContext)
    {
        return Ok();
    }
}