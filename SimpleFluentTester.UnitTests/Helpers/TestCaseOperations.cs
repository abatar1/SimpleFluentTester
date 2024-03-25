using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.UnitTests.TestObjects;

namespace SimpleFluentTester.UnitTests.Helpers;

public static class TestCaseOperations
{
    private static int Operation(int x, int y) => x + y;
        
    private static int ThrowOperation(int _, int __) => throw new Exception();

    private static readonly IComparedObjectFactory ComparedObjectFactory = new ComparedObjectFactory();

    public static ITestSuiteBuilder UseAdderOperation(this ITestSuiteBuilder builder)
    {
        return builder.UseOperation((int x, int y) => x + y);
    }
        
    public static TestCaseOperation Passed
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(3);
            var testCase = new TestCase([1, 2], expected, 1);
            return new TestCaseOperation(Operation, testCase);
        }
    }
        
    public static TestCaseOperation NotPassed
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(4);
            var testCase = new TestCase([1, 2], expected, 1);
            return new TestCaseOperation(Operation, testCase);
        }
    }
        
    public static TestCaseOperation NotPassedWithException
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(3);
            var testCase = new TestCase([1, 2], expected, 1);
            return new TestCaseOperation(ThrowOperation, testCase);
        }
    }
        
    public static TestCaseOperation Invalid
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(4);
            var testCase = new TestCase(["test", 2], expected, 1);
            return new TestCaseOperation(Operation, testCase);
        }
    }
    
    [TestSuiteDelegate]
    // ReSharper disable once UnusedMember.Local
    public static int AdderWithAttribute(int number1, int number2)
    {
        return number1 + number2;
    }
}