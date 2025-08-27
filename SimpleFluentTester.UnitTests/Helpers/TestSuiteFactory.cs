using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Helpers;

internal static class TestSuiteFactory
{
    public static DefinedTestCase DefineTestCaseWithExpectedValue(this ITestSuiteContextContainer container, object?[] inputs, object? expected)
    {
        return container.DefineTestCase(inputs, TestClauseFactory.DefineFromValue(expected));
    }
    
    public static DefinedTestCase DefineTestCase(this ITestSuiteContextContainer container, object?[] inputs, List<DefinedTestClause> clauses)
    {
        var builder = new TestCaseBuilder(container, clauses);
        builder.WithInput(inputs);
        return container.Context.TestCases.Last();
    }

    public static TestSuiteRunResult CreateTestSuiteRunResult(
        Exception? exception = null,
        DefinedTestCase? testCase = null,
        int testCaseToRun = 1,
        bool shouldBeExecuted = true,
        int testCaseNumber = 1)
    {
        var contextContainer = TestSuiteContextContainer.Default(testCaseNumber);

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