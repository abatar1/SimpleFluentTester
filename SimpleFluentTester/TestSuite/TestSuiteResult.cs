using System;
using System.Collections.Generic;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite;

public sealed class TestSuiteResult<TOutput>(
    IList<CompletedTestCase<TOutput>> validatedTestCases,
    IList<ValidationResult> validationResults,
    Delegate? operation,
    string? displayName,
    int number,
    ValidationStatus validationStatus)
{
    public IList<CompletedTestCase<TOutput>> TestCases { get; } = validatedTestCases;

    public ContextValidationResult ContextValidation { get; } = new(validationResults, validationStatus);

    public Delegate? Operation { get; } = operation;

    public string? DisplayName { get; } = displayName;

    public int Number { get; } = number;

    public sealed class ContextValidationResult(
        IList<ValidationResult> validationResults,
        ValidationStatus validationStatus)
    {
        public IList<ValidationResult> Results { get; } = validationResults;
    
        public ValidationStatus Status { get; } = validationStatus;
    }
}