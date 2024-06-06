using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Extensions;

public static class TestSuiteContextContainerExtensions
{
    public static CompletedTestCase CompleteTestCase(
        this ITestSuiteContextContainer contextContainer,
        TestCase testCase,
        params int[] testCasesToRun)
    {
        var operation = testCase.OperationFactory.Invoke();
        if (operation == null)
            throw new InvalidOperationException("Operation should be specified before completing test case. This is a unit test exception, ensure that test written correctly.");
        contextContainer.WithOperation(operation);
        var testCasesHash = new HashSet<int>(testCasesToRun);
        var testCasePipeline = new TestCasePipeline(testCasesHash);
        testCase.RegisterValidation<OperationValidator>(() => new OperationValidationContext(contextContainer.Context.Operation));
        testCase.RegisterValidation<InputsValidator>(() => new InputsValidationContext(contextContainer.Context.Operation));
        return testCasePipeline.ToCompleted(testCase);
    }
}