using System.Collections.Generic;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestRun;

public sealed class ValidatedTestCase<TOutput>(
    ValidationStatus validationStatus, 
    TestCase<TOutput> testCase, 
    IList<ValidationResult> validationResults)
{
    public TestCase<TOutput> TestCase { get; } = testCase;

    public IList<ValidationResult> ValidationResults { get; } = validationResults;

    public ValidationStatus ValidationStatus { get; } = validationStatus;

    public AssertStatus AssertStatus
    {
        get
        {
            var assert = TestCase.Assert;
            if (!assert.IsValueCreated)
                return AssertStatus.Unknown;
            if (!assert.Value.Passed && assert.Value.Exception != null)
                return AssertStatus.NotPassedWithException;
            return assert.Value.Passed ? AssertStatus.Passed : AssertStatus.NotPassed;
        }
    }
}