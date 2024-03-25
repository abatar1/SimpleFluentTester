using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Extensions;

public static class TestSuiteContextContainerExtensions
{
    public static CompletedTestCase CompleteTestCase(
        this ITestSuiteContextContainer contextContainer,
        TestCaseOperation testCaseOperation,
        params int[] testCasesToRun)
    {
        contextContainer.WithOperation(testCaseOperation.Operation);
        var testCasesHash = new HashSet<int>(testCasesToRun);
        var testCasePipeline = new TestCasePipeline(contextContainer.Context, new ComparedObjectFactory(), new ValidationUnpacker(), testCasesHash);
        var testCase = testCaseOperation.TestCase;
        testCase.RegisterValidation<OperationValidator>(() => new OperationValidatedObject(contextContainer.Context.Operation));
        testCase.RegisterValidation<InputsValidator>(() => new InputsValidatedObject(contextContainer.Context.Operation));
        return testCasePipeline.ToCompleted(testCase);
    }
}