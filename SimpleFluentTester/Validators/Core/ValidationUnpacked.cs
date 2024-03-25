using System.Collections.Generic;
using System.Linq;

namespace SimpleFluentTester.Validators.Core;

public sealed class ValidationUnpacked(ValidationStatus status, IList<ValidationResult> results)
{
    public static ValidationUnpacked Empty => new(ValidationStatus.Valid, new List<ValidationResult>());

    public IReadOnlyCollection<ValidationResult> GetNonValid()
    {
        return results
            .Where(x => x.Status != ValidationStatus.Valid)
            .ToList();
    }

    public bool IsValid => status == ValidationStatus.Valid;
}