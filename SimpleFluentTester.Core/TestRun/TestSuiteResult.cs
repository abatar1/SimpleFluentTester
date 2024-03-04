using System.Collections.Generic;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestRun;

public sealed class TestSuiteResult<TOutput>(
    IList<CompletedTestCase<TOutput>> validatedTestCases,
    IList<ValidationResult> contextValidationResults,
    MethodInfo? operationMethodInfo,
    string? displayName,
    int number,
    bool? ignored = null)
{
    public IList<CompletedTestCase<TOutput>> TestCases { get; } = validatedTestCases;

    public IList<ValidationResult> ContextValidationResults { get; } = contextValidationResults;

    public MethodInfo? OperationMethodInfo { get; } = operationMethodInfo;
    
    public bool Ignored { get; } = ignored ?? false;

    public string? DisplayName { get; } = displayName;

    public int Number { get; } = number;
}