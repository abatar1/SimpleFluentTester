using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Represents the result of a test suite execution, including details of the executed test cases,
/// validations performed, and additional metadata about the test suite.
/// </summary>
public sealed class TestSuiteRunResult(
    IList<AssertedTestCase> validatedTestCases,
    Delegate? operation,
    string? displayName,
    int number,
    Exception? exception = null,
    bool shouldBeExecuted = true)
{
    public bool ShouldBeExecuted { get; } = shouldBeExecuted;

    public bool IsValid { get; } = exception == null;

    public Exception? Exception { get; } = exception;
    
    public IList<AssertedTestCase> TestCases { get; } = validatedTestCases;

    public Delegate? Operation { get; } = operation;

    public string? DisplayName { get; } = displayName;

    public int Number { get; } = number;
}
