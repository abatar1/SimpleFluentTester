using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Helpers.TestObjects;

internal sealed class CustomValidator(ValidationSubject validationSubject) 
    : BaseValidator<EmptyValidationContext, EmptyValidatedObject>
{
    public override ValidationSubject Subject => validationSubject;

    protected override ValidationResult ValidateCore(EmptyValidatedObject validatedObject, EmptyValidationContext validationContext)
    {
        return Ok();
    }
}