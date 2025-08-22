using System;

namespace SimpleFluentTester.Validators.Models;

public sealed class ValidationUnexpectedException(string message) : Exception(message);