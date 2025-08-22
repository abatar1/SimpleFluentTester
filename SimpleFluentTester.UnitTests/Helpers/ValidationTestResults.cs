using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Helpers;

public static class ValidationTestResults
{
    public static ValidationResult Valid => ValidationResult.Valid(ValidationSubject.Operation);
        
    public static ValidationResult NonValid => ValidationResult.NonValid(ValidationSubject.Operation, "Non Valid");
    
    public static ValidationResult Failed => ValidationResult.Failed(ValidationSubject.Operation, new Exception(), "Failed");
}