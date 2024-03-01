using System.Collections.Generic;
using System.Reflection;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestRun;

public sealed class TestRunResult<TOutput>(
    IList<ValidatedTestCase<TOutput>> validatedTestCases,
    IList<ValidationResult> contextValidationResults,
    MethodInfo? operationMethodInfo,
    bool? ignored = null)
{
    public IList<ValidatedTestCase<TOutput>> ValidatedTestCases { get; } = validatedTestCases;

    public IList<ValidationResult> ContextValidationResults { get; } = contextValidationResults;

    public MethodInfo? OperationMethodInfo { get; } = operationMethodInfo;
    
    public bool Ignored { get; } = ignored ?? false;
}