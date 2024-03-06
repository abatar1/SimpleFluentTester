using System;

namespace SimpleFluentTester.Validators.Core;

public sealed record ValidationResult(bool IsValid, ValidationSubject ValidationSubject, string? Message = null, Exception? Exception = null)
{
    public static ValidationResult Failed(ValidationSubject validationSubject, string? message = null, Exception? exception = null)
    {
        return new ValidationResult(false, validationSubject, message, exception);
    }
    
    public static ValidationResult Ok(ValidationSubject validationSubject)
    {
        return new ValidationResult(true, validationSubject);
    }

    public bool IsValid { get; } = IsValid;

    public ValidationSubject ValidationSubject { get; } = ValidationSubject;
    
    public string? Message { get; } = Message;
    
    public Exception? Exception { get; } = Exception;
}