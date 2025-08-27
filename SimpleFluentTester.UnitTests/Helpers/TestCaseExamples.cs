using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Helpers;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Helpers;

internal static class TestCaseExamples
{
    public static ITestSuiteBuilder UseAdderOperation(this ITestSuiteBuilder builder)
    {
        return builder.UseOperation((int x, int y) => x + y);
    }
        
    public static DefinedTestCase Passed
    {
        get
        {
            var expected = TestClauseFactory.DefineFromValue(3);
            return new DefinedTestCase(new Lazy<Delegate?>(() => Operation) , new Lazy<Delegate?>(() => Comparer),ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
        }
    }
        
    public static DefinedTestCase NotPassed
    {
        get
        {
            var expected = TestClauseFactory.DefineFromValue(4);
            return new DefinedTestCase(new Lazy<Delegate?>(() => Operation), new Lazy<Delegate?>(() => Comparer),ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
        }
    }
        
    public static DefinedTestCase NotPassedWithOperationException
    {
        get
        {
            var expected = TestClauseFactory.DefineFromValue(3);
            return new DefinedTestCase(new Lazy<Delegate?>(() => ThrowOperation), new Lazy<Delegate?>(() => Comparer),ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
        }
    }
    
    public static DefinedTestCase NotPassedWithComparerException
    {
        get
        {
            var expected = TestClauseFactory.DefineFromValue(3);
            return new DefinedTestCase(new Lazy<Delegate?>(() => Operation), new Lazy<Delegate?>(() => ThrowComparer),ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
        }
    }
        
    public static DefinedTestCase NonValidOperation
    {
        get
        {
            var expected = TestClauseFactory.DefineFromValue(3);
            var testCase = new DefinedTestCase(new Lazy<Delegate?>(() => Operation), new Lazy<Delegate?>(() => Comparer), ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
            testCase.AddReadyValidation(ValidationResult.NonValid(ValidationSubject.Operation, NonValidOperationMessage));
            return testCase;
        }
    }
    
    public static string NonValidOperationMessage => "Test";
    
    [TestSuiteDelegate]
    // ReSharper disable once UnusedMember.Local
    public static int AdderWithAttribute(int number1, int number2)
    {
        return number1 + number2;
    }
    
    private static int Operation(int x, int y) => x + y;
    
    private static bool Comparer(int x, int y) => x == y;
    
    private static int ThrowOperation(int _, int __) => throw new Exception();
    
    private static bool ThrowComparer(int _, int __) => throw new Exception();
}