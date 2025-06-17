using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Helpers.TestObjects;

public sealed class CustomValidatedObject(IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> validations)
    : IValidatedObject
{
    public IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> Validations { get; } = validations;
}