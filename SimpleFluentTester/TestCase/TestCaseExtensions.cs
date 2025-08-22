using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase;

public static class TestCaseExtensions
{
    public static bool IsValid(this ITestCase testCase)
    {
        return testCase.Validations.Values
            .SelectMany(x => x)
            .SelectMany(x => x.Value.Validations)
            .All(x => x.Status == ValidationStatus.Valid);
    }

    public static IReadOnlyCollection<ValidationResult> GetInvalid(this IDictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> validations)
    {
        return validations.Values.SelectMany(x => x)
            .SelectMany(x => x.Value.Validations)
            .Where(x => x.Status != ValidationStatus.Valid)
            .ToList();
    }
}