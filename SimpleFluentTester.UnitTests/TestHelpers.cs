using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests;

public static class TestHelpers
{
    public static ITestSuiteBuilderContext<TOutput> CreateEmptyContext<TOutput>(
        IEntryAssemblyProvider? assemblyProvider = null,
        IActivator? activator = null,
        Delegate? operation = null)
    {
        return new TestSuiteBuilderContext<TOutput>(
            0,
            "TestSuite",
            assemblyProvider ?? new EntryAssemblyProvider(),
            activator ?? new DefaultActivator(),
            new List<TestCase<TOutput>>(),
            operation,
            null,
            new Dictionary<ValidationSubject, IList<ValidationResult>>(),
            true);
    }

    internal static int Adder(int number1, int number2)
    {
        return number1 + number2;
    }
}

internal class CustomException : Exception;