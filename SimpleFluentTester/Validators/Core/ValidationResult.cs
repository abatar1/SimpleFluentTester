using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Core;

public sealed record ValidationResult(ValidationStatus Status, ValidationSubject Subject, string? Message = null)
{
    public ValidationStatus Status { get; } = Status;

    public ValidationSubject Subject { get; } = Subject;

    public string? Message { get; } = Message;

    public bool IsValid => Status is ValidationStatus.Valid or ValidationStatus.Skipped;

    public static ValidationResult Failed(ValidationSubject subject, string? message = null)
    {
        return new ValidationResult(ValidationStatus.NonValid, subject, message);
    }
    
    public static ValidationResult Ok(ValidationSubject subject)
    {
        return new ValidationResult(ValidationStatus.Valid, subject);
    }
    
    public static ValidationResult Skipped(ValidationSubject subject)
    {
        return new ValidationResult(ValidationStatus.Skipped, subject);
    }
}