using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class CustomValidated(IDictionary<ValidationSubject, IList<Func<ValidationResult>>> validations)
    : IValidated
{
    public IDictionary<ValidationSubject, IList<Func<ValidationResult>>> Validations { get; } = validations;
}