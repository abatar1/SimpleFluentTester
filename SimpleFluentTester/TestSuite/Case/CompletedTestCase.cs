using System;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

/// <summary>
/// Represents <see cref="TestCase"/> after proceeding by <see cref="TestCasePipeline"/>. Contains validation results of TestCase,
/// assertion result, elapsed execution time. Could be ignored using <see cref="CompletedTestCase.NotExecuted"/>
/// </summary>
public sealed class CompletedTestCase
{
    internal CompletedTestCase(AssertResult assertResult,
        TimeSpan elapsedTime,
        PackedValidation packedValidation, 
        TestCase testCase)
    {
        Number = testCase.Number;
        Assert = assertResult;
        ElapsedTime = elapsedTime;
        Inputs = testCase.Inputs;
        Expected = testCase.Expected;
        Validation = packedValidation;
    }

    public int Number { get; }
    
    public AssertResult Assert { get; }
    
    public TimeSpan ElapsedTime { get; }
    
    public IComparedObject[] Inputs { get; }

    public IComparedObject Expected { get; }

    public PackedValidation Validation { get; }

    public static CompletedTestCase NotExecuted(TestCase testCase)
    {
        return new CompletedTestCase(
            AssertResult.Ignored,
            TimeSpan.Zero,
            PackedValidation.Empty,
            testCase); 
    }
}
