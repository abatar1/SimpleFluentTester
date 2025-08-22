using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Helpers.TestObjects;

public sealed class CustomValidatedObject(IDictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> validations)
    : IValidatedObject
{
    public IDictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> Validations { get; } = validations;
}