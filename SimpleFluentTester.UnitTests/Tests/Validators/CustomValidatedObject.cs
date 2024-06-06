using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class CustomValidatedObject(IDictionary<ValidationSubject, IList<Func<ValidationResult>>> validations)
    : IValidatedObject
{
    public IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; } = validations;
}