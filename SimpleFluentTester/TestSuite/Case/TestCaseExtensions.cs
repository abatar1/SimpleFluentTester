using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

public static class TestCaseExtensions
{
    public static bool IsValid(this IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> validations)
    {
        return validations.Values.SelectMany(x => x).All(x => x.Value.Status == ValidationStatus.Valid);
    }

    public static IReadOnlyCollection<ValidationResult> GetInvalid(this IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> validations)
    {
        return validations.Values.SelectMany(x => x)
            .Select(x => x.Value)
            .Where(x => x.Status != ValidationStatus.Valid)
            .ToList();
    }
}