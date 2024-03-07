using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
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

    public static TestSuiteResult<TOutput> GetTestSuiteResult<TOutput>(
        ValidationResult contextValidation,
        Delegate operation,
        TestCase<TOutput> testCase)
    {
        var validationResults = new Dictionary<ValidationSubject, IList<ValidationResult>>
        {
            {
                contextValidation.Subject,
                new List<ValidationResult> { contextValidation }
            }
        };
        var context = CreateEmptyContext<TOutput>(operation: operation);
        var testCaseExecutor = new TestCaseExecutor<TOutput>(context);
        var completedTestCase = testCaseExecutor.TryCompeteTestCase(testCase, new SortedSet<int>([1]));
        return new TestSuiteResult<TOutput>(
            new List<CompletedTestCase<TOutput>> { completedTestCase },
            validationResults,
            context.Operation,
            context.Name,
            context.Number);
    }

    internal static int Adder(int number1, int number2)
    {
        return number1 + number2;
    }
}

internal class CustomException : Exception;