using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.UnitTests.Extensions;

public static class CompletedTestCaseExtensions
{
    public static void AssertPassed<TExpected>(
        this CompletedTestCase testCase,
        TExpected? expected,
        object?[] inputs,
        Func<TExpected?, TExpected?, bool>? comparer = null)
    {
        testCase.Validation.AssertValid();

        Assert.Equal(AssertStatus.Passed, testCase.Assert.Status);
        
        testCase.AssertOutput(expected, inputs, true, comparer);
    }
    
    public static void AssertFailed(
        this CompletedTestCase testCase,
        Exception exception,
        string message)
    {
        testCase.Validation.AssertValid();

        Assert.Equal(AssertStatus.Failed, testCase.Assert.Status);
        
        Assert.NotNull(testCase.Assert.Output);
        Assert.NotNull(testCase.Expected);
        Assert.NotNull(testCase.Assert.Exception);
        Assert.Equal(exception.GetType(), testCase.Assert.Exception.GetType());
        Assert.NotNull(testCase.Assert.Message);
        Assert.Equal(message, testCase.Assert.Message);
    }

    public static void AssertNotPassed<TExpected>(
        this CompletedTestCase testCase,
        TExpected? expected,
        object?[] inputs,
        Func<TExpected?, TExpected?, bool>? comparer = null)
    {
        testCase.Validation.AssertValid();

        Assert.Equal(AssertStatus.NotPassed, testCase.Assert.Status);
        
        testCase.AssertOutput(expected, inputs, false, comparer);
    }

    public static void AssertNotPassedWithException<TExpected>(
        this CompletedTestCase testCase,
        TExpected? expected,
        object?[] inputs,
        Type exceptionType,
        Func<TExpected?, TExpected?, bool>? comparer = null)
    {
        testCase.Validation.AssertValid();

        Assert.Equal(AssertStatus.NotPassedWithException, testCase.Assert.Status);

        Assert.Equal(ComparedObjectVariety.Exception, testCase.Assert.Output.Variety);
        Assert.Equal(exceptionType, testCase.Assert.Output.Type);
        Assert.Equal(exceptionType, testCase.Assert.Output.Value?.GetType());

        testCase.AssertOutput(expected, inputs, false, comparer);
    }
    
    public static void AssertSkippedTestResult(
        this CompletedTestCase testCase,
        object? expected,
        object?[] inputs)
    {
        testCase.Validation.AssertValid();

        Assert.Equal(AssertStatus.Ignored, testCase.Assert.Status);
        
        testCase.AssertNullOutput();
        
        Assert.Equal(expected, testCase.Expected.Value);
        Assert.Equal(inputs, testCase.Inputs.Select(x => x.Value));
    }

    private static void AssertOutput<TExpected>(
        this CompletedTestCase testCase,
        TExpected? expectedResult,
        IEnumerable<object?> expectedInputs,
        bool shouldBeEqual,
        Func<TExpected?, TExpected?, bool>? comparer)
    {
        Assert.NotNull(testCase.Assert.Output);
        if (testCase.Assert.Output.Variety != ComparedObjectVariety.Null)
            Assert.NotNull(testCase.Assert.Output.Value);
        
        if (testCase.Assert.Output.Variety == ComparedObjectVariety.Null && testCase.Expected.Variety == ComparedObjectVariety.Null)
            return;
        
        if (testCase.Expected.Value is Exception expectedException)
        {
            if (testCase.Assert.Output.Value is not Exception outputException)
            {
                Assert.Fail("Output is not Exception, however the expected is.");
                return;
            }
            
            Assert.Equal(shouldBeEqual, expectedException.GetType() == outputException.GetType());
            Assert.Equal(shouldBeEqual, expectedException.Message == outputException.Message);
            return;
        }
        
        if (comparer == null)
        {
            Assert.Equal(shouldBeEqual, expectedResult?.Equals(testCase.Assert.Output.Value));
            Assert.Equal(shouldBeEqual, testCase.Expected.Value?.Equals(testCase.Assert.Output.Value));
            Assert.Equal(expectedInputs, testCase.Inputs.Select(x => x.Value));
            return;
        }

        Assert.Equal(shouldBeEqual, comparer.Invoke((TExpected?)testCase.Assert.Output.Value, (TExpected?)testCase.Expected.Value));
        Assert.Equal(shouldBeEqual, comparer.Invoke(expectedResult, (TExpected?)testCase.Assert.Output.Value));
    }
    
    private static void AssertNullOutput(this CompletedTestCase testCase)
    {
        Assert.NotNull(testCase.Assert.Output);
        Assert.Null(testCase.Assert.Output.Value);
        Assert.Null(testCase.Assert.Output.Type);
        Assert.Equal(ComparedObjectVariety.Null, testCase.Assert.Output.Variety);
    }
}