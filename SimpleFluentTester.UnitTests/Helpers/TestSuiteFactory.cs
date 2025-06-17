using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Helpers;

internal static class TestSuiteFactory
{
    public static DeferredTestCase CreateDeferredTestCase(ITestSuiteContextContainer container, object?[] inputs, object? expected)
    {
        var testCase = new DeferredTestCase(
            new Lazy<Delegate?>(() => container.Context.Operation),
            new Lazy<Delegate?>(() => container.Context.Comparer),
            ComparedObjectFactory.WrapMany(inputs), 
            ComparedObjectFactory.Wrap(expected), 
            1);
        container.Context.TestCases.Add(testCase);
        return testCase;
    }

    public static ITestSuiteContextContainer CreateEmptyContextContainer(
        IEntryAssemblyProvider? assemblyProvider = null,
        IActivator? activator = null,
        Delegate? operation = null,
        int testSuiteNumber = 1,
        bool? shouldBeExecuted = true)
    {
        var defaultContainer = TestSuiteContextContainer.Default(testSuiteNumber);
        var defaultContext = defaultContainer.Context;
        var context = new TestSuiteContext(
            defaultContext.Number,
            defaultContext.Name,
            assemblyProvider ?? defaultContext.EntryAssemblyProvider,
            activator ?? defaultContext.Activator,
            defaultContext.TestCases,
            operation,
            null,
            shouldBeExecuted ?? defaultContext.ShouldBeExecuted);
        return new TestSuiteContextContainer(context);
    }

    public static TestSuiteRunResult CreateTestSuiteRunResult(
        Exception? exception = null,
        DeferredTestCase? testCase = null,
        int testCaseToRun = 1,
        bool shouldBeExecuted = true,
        int testCaseNumber = 1)
    {
        var contextContainer =
            CreateEmptyContextContainer(testSuiteNumber: testCaseNumber, shouldBeExecuted: shouldBeExecuted);

        var completedTestCases = new List<AssertedTestCase>();
        if (testCase != null)
        {
            var completedTestCase = testCase.CompleteTestCase(contextContainer, testCaseToRun);
            completedTestCases.Add(completedTestCase);
        }

        return new TestSuiteRunResult(
            completedTestCases,
            contextContainer.Context.Operation,
            contextContainer.Context.Name,
            contextContainer.Context.Number,
            exception,
            shouldBeExecuted);
    }
}