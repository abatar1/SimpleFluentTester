using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.UnitTests;

public static class TestHelpers
{
    public static ITestSuiteBuilderContext<TOutput> CreateEmptyContext<TOutput>(IEntryAssemblyProvider? assemblyProvider = null)
    {
        assemblyProvider ??= new EntryAssemblyProvider();
        return new TestSuiteBuilderContext<TOutput>(
            0,
            "TestSuite",
            assemblyProvider, 
            new DefaultActivator(),
            new List<TestCase<TOutput>>(), 
            new ValueWrapper<Delegate>(), 
            null, 
            true);
    }
    
    internal static int Adder(int number1, int number2)
    {
        return number1 + number2;
    }
}

internal class CustomException : Exception;