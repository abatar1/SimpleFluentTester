using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Helpers;

public static class TestSuiteFactory
{
    public static TestCase CreateAndAddTestCase(ITestSuiteContextContainer container, object?[] inputs, object? expected)
    {
        var testCase = new TestCase(
            () => container.Context.Operation,
            () => container.Context.Comparer,
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
            defaultContext.Validations,
            shouldBeExecuted ?? defaultContext.ShouldBeExecuted);
        return new TestSuiteContextContainer(context);
    }

    public static TestSuiteRunResult CreateTestSuiteRunResult(
        ValidationResult? validationResult = null,
        TestCase? testCase = null,
        int testCaseToRun = 1,
        bool shouldBeExecuted = true,
        int testCaseNumber = 1)
    {
        var contextContainer =
            CreateEmptyContextContainer(testSuiteNumber: testCaseNumber, shouldBeExecuted: shouldBeExecuted);
        if (validationResult != null)
            contextContainer.Context.AddValidation(validationResult);

        var completedTestCases = new List<CompletedTestCase>();
        if (testCase != null)
        {
            var completedTestCase = testCase.CompleteTestCase(contextContainer, testCaseToRun);
            completedTestCases.Add(completedTestCase);
        }

        return new TestSuiteRunResult(
            completedTestCases,
            ValidationPipe.ValidatePacked(contextContainer.Context),
            contextContainer.Context.Operation,
            contextContainer.Context.Name,
            contextContainer.Context.Number,
            shouldBeExecuted);
    }
}