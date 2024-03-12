using System.Collections.Generic;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase;

public sealed class CompletedTestCase<TOutput>(
    Assert<TOutput>? assert,
    ValidationStatus validationStatus, 
    TestCase<TOutput> testCase, 
    IList<ValidationResult> validationResults)
{
    public Assert<TOutput>? Assert { get; } = assert;
    
    public object?[] Inputs { get; } = testCase.Inputs;
    
    public TOutput? Expected { get; } = testCase.Expected;
    
    public int Number { get; } = testCase.Number;

    public bool Executed => Assert != null;

    public IList<ValidationResult> ValidationResults { get; } = validationResults;

    public ValidationStatus ValidationStatus { get; } = validationStatus;

    public static CompletedTestCase<TOutput> NotExecuted(TestCase<TOutput> testCase)
    {
        return new CompletedTestCase<TOutput>(null, ValidationStatus.Ignored, testCase, new List<ValidationResult>()); 
    }
}