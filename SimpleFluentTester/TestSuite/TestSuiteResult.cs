using System;
using System.Collections.Generic;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite;

public sealed class TestSuiteResult<TOutput>(
    IList<CompletedTestCase<TOutput>> validatedTestCases,
    IDictionary<ValidationSubject, IList<ValidationResult>> validationResults,
    Delegate? operation,
    string? displayName,
    int number,
    bool? ignored = null)
{
    public IList<CompletedTestCase<TOutput>> TestCases { get; } = validatedTestCases;

    public IDictionary<ValidationSubject, IList<ValidationResult>> ValidationResults { get; } = validationResults;

    public Delegate? Operation { get; } = operation;
    
    public bool Ignored { get; } = ignored ?? false;

    public string? DisplayName { get; } = displayName;

    public int Number { get; } = number;
}