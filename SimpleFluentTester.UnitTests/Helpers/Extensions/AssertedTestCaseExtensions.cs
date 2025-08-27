using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Helpers.Extensions;

internal static class AssertedTestCaseExtensions
{
    public static void AssertPassed<TExpected>(
        this AssertedTestCase testCase,
        TExpected? expected,
        object?[] inputs,
        Func<TExpected?, TExpected?, bool>? comparer = null)
    {
        testCase.AssertValid();
        var testClause = (AssertedTestClause)testCase.Clauses.First();
        Assert.Equal(AssertStatus.Passed, testClause.Assert.Status);
        testCase.AssertOutput(expected, inputs, true, comparer);
    }
    
    public static void AssertFailedValidation<TException>(
        this AssertedTestCase testCase,
        ValidationSubject validationSubject,
        string message,
        string? innerMessage = null)
    where TException : Exception
    {
        AssertValidationExtensions.AssertFailedValidation<TException>(testCase, validationSubject, message, innerMessage);
        
        var testClause = (AssertedTestClause)testCase.Clauses.First();
        Assert.Equal(AssertStatus.Failed, testClause.Assert.Status);
        Assert.NotNull(testClause.Result);
        Assert.NotNull(testClause.Expected);
        Assert.NotNull(testClause.Assert.Exception);
        Assert.Equal(typeof(TException), testClause.Assert.Exception.GetType());
        Assert.NotNull(testClause.Assert.Message);
        
        if (innerMessage != null)
            Assert.Equal(innerMessage, testClause.Assert.Message);
    }

    public static void AssertNotPassed<TExpected>(
        this AssertedTestCase testCase,
        TExpected? expected,
        object?[] inputs,
        Func<TExpected?, TExpected?, bool>? comparer = null)
    {
        testCase.AssertValid();
        testCase.AssertOutput(expected, inputs, false, comparer);
        
        var testClause = (AssertedTestClause)testCase.Clauses.First();
        Assert.Equal(AssertStatus.NotPassed, testClause.Assert.Status);
    }

    public static void AssertNotPassedWithException<TExpected>(
        this AssertedTestCase testCase,
        TExpected? expected,
        object?[] inputs,
        Type exceptionType,
        Func<TExpected?, TExpected?, bool>? comparer = null)
    {
        testCase.AssertValid();
        testCase.AssertOutput(expected, inputs, false, comparer);
        
        var testClause = (AssertedTestClause)testCase.Clauses.First();
        Assert.Equal(AssertStatus.NotPassedWithException, testClause.Assert.Status);
        Assert.Equal(ComparedObjectVariety.Exception, testClause.Result.Variety);
        Assert.Equal(exceptionType, testClause.Result.Type);
        Assert.Equal(exceptionType, testClause.Result.Value?.GetType());
    }
    
    public static void AssertIgnored(this AssertedTestCase testCase)
    {
        var testClause = (AssertedTestClause)testCase.Clauses.First();
        Assert.NotNull(testClause.Result);
        Assert.Null(testClause.Result.Value);
        Assert.Null(testClause.Result.Type);
        Assert.Equal(ComparedObjectVariety.Null, testClause.Result.Variety);
        Assert.Equal(AssertStatus.Ignored, testClause.Assert.Status);
    }
    
    public static void AssertFailed(this AssertedTestCase testCase)
    {
        var testClause = (AssertedTestClause)testCase.Clauses.First();
        Assert.NotNull(testClause.Result);
        Assert.Null(testClause.Result.Value);
        Assert.Null(testClause.Result.Type);
        Assert.Equal(ComparedObjectVariety.Null, testClause.Result.Variety);
        Assert.Equal(AssertStatus.Failed, testClause.Assert.Status);
    }

    private static void AssertOutput<TExpected>(
        this AssertedTestCase testCase,
        TExpected? expectedResult,
        IEnumerable<object?> expectedInputs,
        bool shouldBeEqual,
        Delegate? comparer)
    {
        var testClause = (AssertedTestClause)testCase.Clauses.First();
        Assert.NotNull(testClause.Result);
        if (testClause.Result.Variety != ComparedObjectVariety.Null)
            Assert.NotNull(testClause.Result.Value);
        
        if (testClause.Result.Variety == ComparedObjectVariety.Null && testClause.Expected.Variety == ComparedObjectVariety.Null)
            return;
        
        if (testClause.Expected.Value is Exception expectedException)
        {
            if (testClause.Result.Value is not Exception outputException)
            {
                Assert.Fail("Output is not Exception, however the expected is.");
                return;
            }
            
            Assert.Equal(shouldBeEqual, expectedException.Message == outputException.Message);
            Assert.Equal(shouldBeEqual, expectedException.GetType() == outputException.GetType());
            return;
        }
        
        if (comparer == null)
        {
            Assert.Equal(shouldBeEqual, expectedResult?.Equals(testClause.Result.Value) ?? testClause.Result.Value == null);;
            Assert.Equal(shouldBeEqual, testClause.Expected.Value?.Equals(testClause.Result.Value) ?? testClause.Result.Value == null);;
            Assert.Equal(expectedInputs, testCase.Inputs.Select(x => x.Value));
            return;
        }

        if (testClause.Expected.Variety == ComparedObjectVariety.Parameter)
        {
            var value = ObjectParameterExtractor.ExtractExpectedParameterValue(testClause);
            Assert.Equal(shouldBeEqual, comparer.Method.Invoke(comparer.Target, [expectedResult, value]));
            return;
        }
        
        Assert.Equal(shouldBeEqual, comparer.Method.Invoke(comparer.Target, [(TExpected?)testClause.Result.Value, (TExpected?)testClause.Expected.Value]));
        Assert.Equal(shouldBeEqual, comparer.Method.Invoke(comparer.Target, [expectedResult, (TExpected?)testClause.Result.Value]));
    }
}