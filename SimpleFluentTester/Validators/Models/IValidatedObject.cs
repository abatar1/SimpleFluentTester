using System;
using System.Collections.Generic;

namespace SimpleFluentTester.Validators.Models;

/// <summary>
/// Defines a contract for objects that support validation logic through a collection of validation operations.
/// </summary>
/// <remarks>
/// This interface is designed to provide a mechanism for associating a set of validation operations
/// with their respective subjects. Each validation operation is represented as a pair of a validation
/// subject and a lazy-loaded validation result or function.
/// The <see cref="IValidatedObject"/> interface is typically implemented by objects that require
/// validation logic, allowing them to expose and operate on their validation rules.
/// </remarks>
public interface IValidatedObject
{
    IDictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> Validations { get; }
}