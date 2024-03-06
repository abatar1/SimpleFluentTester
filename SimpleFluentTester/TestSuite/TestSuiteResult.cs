using System;
using System.Collections.Generic;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite;

public sealed class TestSuiteResult<TOutput>(
    IList<CompletedTestCase<TOutput>> validatedTestCases,
    IList<ValidationResult> contextValidationResults,
    ValueWrapper<Delegate> operation,
    string? displayName,
    int number,
    bool? ignored = null)
{
    public IList<CompletedTestCase<TOutput>> TestCases { get; } = validatedTestCases;

    public IList<ValidationResult> ContextValidationResults { get; } = contextValidationResults;

    public  ValueWrapper<Delegate> Operation { get; } = operation;
    
    public bool Ignored { get; } = ignored ?? false;

    public string? DisplayName { get; } = displayName;

    public int Number { get; } = number;
}