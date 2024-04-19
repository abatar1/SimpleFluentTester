using System;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

public sealed class CompletedTestCase
{
    internal CompletedTestCase(AssertResult assertResult,
        TimeSpan elapsedTime,
        ValidationUnpacked validationUnpacked, 
        TestCase testCase)
    {
        Number = testCase.Number;
        Assert = assertResult;
        ElapsedTime = elapsedTime;
        Inputs = testCase.Inputs;
        Expected = testCase.Expected;
        Validation = validationUnpacked;
    }

    public int Number { get; }
    
    public AssertResult Assert { get; }
    
    public TimeSpan ElapsedTime { get; }
    
    public IComparedObject[] Inputs { get; }

    public IComparedObject Expected { get; }

    public ValidationUnpacked Validation { get; }

    public static CompletedTestCase NotExecuted(TestCase testCase)
    {
        return new CompletedTestCase(
            AssertResult.Ignored,
            TimeSpan.Zero,
            ValidationUnpacked.Empty,
            testCase); 
    }
}
