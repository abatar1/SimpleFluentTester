using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite;

public sealed class TestSuiteResult(
    IList<CompletedTestCase> validatedTestCases,
    ValidationUnpacked validation,
    Delegate? operation,
    string? displayName,
    int number,
    bool shouldBeExecuted = true)
{
    public bool ShouldBeExecuted { get; } = shouldBeExecuted;
    
    public IList<CompletedTestCase> TestCases { get; } = validatedTestCases;

    public ValidationUnpacked Validation { get; } = validation;

    public Delegate? Operation { get; } = operation;

    public string? DisplayName { get; } = displayName;

    public int Number { get; } = number;
}