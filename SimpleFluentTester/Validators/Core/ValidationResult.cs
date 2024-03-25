using System;

namespace SimpleFluentTester.Validators.Core;

public sealed record ValidationResult
{
    private ValidationResult(ValidationStatus status, ValidationSubject subject, string? message = null, Exception? exception = null)
    {
        Status = status;
        Subject = subject;
        Message = message;
        Exception = exception;
    }

    public ValidationStatus Status { get; }

    public ValidationSubject Subject { get; }

    public string? Message { get; }

    public Exception? Exception { get; }
    
    public static ValidationResult Failed(ValidationSubject subject, Exception exception, string message)
    {
        return new ValidationResult(ValidationStatus.Failed, subject, message, exception);
    }

    public static ValidationResult NonValid(ValidationSubject subject, string message)
    {
        return new ValidationResult(ValidationStatus.NonValid, subject, message);
    }
    
    public static ValidationResult Valid(ValidationSubject subject)
    {
        return new ValidationResult(ValidationStatus.Valid, subject);
    }

    public static ValidationResult FromStatus(ValidationStatus status, ValidationSubject subject, string? message = null, Exception? exception = null)
    {
        return new ValidationResult(status, subject, message, exception);
    }
}