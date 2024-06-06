using System.Collections.Generic;
using System.Linq;

namespace SimpleFluentTester.Validators.Core;

public sealed class PackedValidation(IValidatedObject validatedObject, IList<ValidationResult> results, ValidationStatus status)
{
    public IValidatedObject ValidatedObject { get; } = validatedObject;
    
    public static PackedValidation Empty => new(new EmptyValidatedObject(), new List<ValidationResult>(), ValidationStatus.Valid);

    public IReadOnlyCollection<ValidationResult> GetNonValid()
    {
        return results
            .Where(x => x.Status != ValidationStatus.Valid)
            .ToList();
    }

    public bool IsValid => status == ValidationStatus.Valid;
}