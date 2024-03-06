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

    public IList<ValidationResult> ValidationResults { get; } = validationResults;

    public ValidationStatus ValidationStatus { get; } = validationStatus;

    public AssertStatus AssertStatus
    {
        get
        {
            if (Assert == null)
                return AssertStatus.Unknown;
            if (Assert is { Passed: false, Exception: not null })
                return AssertStatus.NotPassedWithException;
            return Assert.Passed ? AssertStatus.Passed : AssertStatus.NotPassed;
        }
    }

    public static CompletedTestCase<TOutput> NotExecuted(TestCase<TOutput> testCase)
    {
        return new CompletedTestCase<TOutput>(null, ValidationStatus.Unknown, testCase, new List<ValidationResult>()); 
    }
}