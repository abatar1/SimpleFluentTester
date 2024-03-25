using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

internal sealed class CustomValidator(ValidationSubject validationSubject) 
    : BaseValidator<EmptyValidatedObject>
{
    public override ISet<Type> AllowedTypes => new HashSet<Type>([ValidatedTypes.Context]);
    public override ValidationSubject Subject => validationSubject;

    public override ValidationResult Validate(IValidated validated, IValidatedObject validatedObject)
    {
        return Ok();
    }
}