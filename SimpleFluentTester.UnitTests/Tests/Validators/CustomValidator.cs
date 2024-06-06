using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

internal sealed class CustomValidator(ValidationSubject validationSubject) 
    : BaseValidator<EmptyValidationContext, EmptyValidatedObject>
{
    public override Type AllowedType => ValidatedTypes.Context;
    
    public override ValidationSubject Subject => validationSubject;

    protected override ValidationResult ValidateCore(EmptyValidatedObject validatedObject, EmptyValidationContext validationContext)
    {
        return Ok();
    }
}