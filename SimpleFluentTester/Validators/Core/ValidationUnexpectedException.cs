using System;

namespace SimpleFluentTester.Validators.Core;

public sealed class ValidationUnexpectedException : Exception
{
    public ValidationUnexpectedException()
    {
    }

    public ValidationUnexpectedException(string message) : base(message)
    {
    }

    public ValidationUnexpectedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}