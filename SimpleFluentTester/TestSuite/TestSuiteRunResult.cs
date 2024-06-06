using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite;

public sealed class TestSuiteRunResult(
    IList<CompletedTestCase> validatedTestCases,
    PackedValidation validation,
    Delegate? operation,
    string? displayName,
    int number,
    bool shouldBeExecuted = true)
{
    public bool ShouldBeExecuted { get; } = shouldBeExecuted;
    
    public IList<CompletedTestCase> TestCases { get; } = validatedTestCases;

    public PackedValidation Validation { get; } = validation;

    public Delegate? Operation { get; } = operation;

    public string? DisplayName { get; } = displayName;

    public int Number { get; } = number;
}