using System;
using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Models;

/// <summary>
/// Represents an empty implementation of the <see cref="IValidatedObject"/> interface.
/// </summary>
/// <remarks>
/// The <see cref="EmptyValidatedObject"/> class provides an empty implementation for scenarios where
/// no specific validation logic is required. This can serve as a placeholder or default object
/// during validation processes.
/// </remarks>
public sealed class EmptyValidatedObject : IValidatedObject
{
    public IDictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> Validations { get; } =
        new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>();
}