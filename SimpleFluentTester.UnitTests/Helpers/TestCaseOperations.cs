using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.UnitTests.Helpers;

public static class TestCaseOperations
{
    private static int Operation(int x, int y) => x + y;
    
    private static bool Comparer(int x, int y) => x == y;
        
    private static int ThrowOperation(int _, int __) => throw new Exception();
    
    private static bool ThrowComparer(int _, int __) => throw new Exception();

    public static ITestSuiteBuilder UseAdderOperation(this ITestSuiteBuilder builder)
    {
        return builder.UseOperation((int x, int y) => x + y);
    }
        
    public static TestCase Passed
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(3);
            return new TestCase(() => Operation, () => Comparer,ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
        }
    }
        
    public static TestCase NotPassed
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(4);
            return new TestCase(() => Operation, () => Comparer,ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
        }
    }
        
    public static TestCase NotPassedWithOperationException
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(3);
            return new TestCase(() => ThrowOperation, () => Comparer,ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
        }
    }
    
    public static TestCase NotPassedWithComparerException
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(3);
            return new TestCase(() => Operation, () => ThrowComparer,ComparedObjectFactory.WrapMany([1, 2]), expected, 1);
        }
    }
        
    public static TestCase Invalid
    {
        get
        {
            var expected = ComparedObjectFactory.Wrap(4);
            return new TestCase(() => Operation, () => Comparer,ComparedObjectFactory.WrapMany(["test", 2]), expected, 1);
        }
    }
    
    [TestSuiteDelegate]
    // ReSharper disable once UnusedMember.Local
    public static int AdderWithAttribute(int number1, int number2)
    {
        return number1 + number2;
    }
}