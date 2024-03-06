using System;

namespace SimpleFluentTester.Validators.Core;

public sealed class ValidationUnexpectedException(string message) : Exception(message);