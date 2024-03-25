using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.TestObjects;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Helpers;

public static class TestSuiteFactory
{
    private static readonly ComparedObjectFactory ComparedObjectFactory = new();

    public static TestCase CreateTestCase(object?[] inputs, object? expected)
    {
        return new TestCase(inputs, ComparedObjectFactory.Wrap(expected), 1);
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
            defaultContext.Validations,
            shouldBeExecuted ?? defaultContext.ShouldBeExecuted);
        return new TestSuiteContextContainer(context);
    }

    public static TestSuiteResult CreateTestSuiteResult(
        ValidationResult? validationResult = null,
        TestCaseOperation? testCaseOperation = null,
        int testCaseToRun = 1,
        bool shouldBeExecuted = true,
        int testCaseNumber = 1)
    {
        var contextContainer =
            CreateEmptyContextContainer(testSuiteNumber: testCaseNumber, shouldBeExecuted: shouldBeExecuted);
        if (validationResult != null)
            contextContainer.Context.AddValidation(validationResult);

        var completedTestCases = new List<CompletedTestCase>();
        if (testCaseOperation != null)
        {
            var completedTestCase = contextContainer.CompleteTestCase(testCaseOperation, testCaseToRun);
            completedTestCases.Add(completedTestCase);
        }

        return new TestSuiteResult(
            completedTestCases,
            new ValidationUnpacker().Unpack(contextContainer.Context),
            contextContainer.Context.Operation,
            contextContainer.Context.Name,
            contextContainer.Context.Number,
            shouldBeExecuted);
    }
}