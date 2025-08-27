using System.Collections.Generic;
using System.Linq;

namespace SimpleFluentTester.Validators.Models;

public record SubjectValidation
{
    public SubjectValidation(IList<ValidationResult> validations)
    {
        Validations = validations;
    }
    
    public SubjectValidation(IList<SubjectValidation> validations)
    {
        Validations = validations.SelectMany(x => x.Validations).ToList();
    }
    
    public IList<ValidationResult> Validations { get; }
}