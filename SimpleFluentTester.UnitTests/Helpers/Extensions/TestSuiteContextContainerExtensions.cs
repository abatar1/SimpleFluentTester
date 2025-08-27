using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Pipeline;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Helpers;

namespace SimpleFluentTester.UnitTests.Helpers.Extensions;

internal static class TestSuiteContextContainerExtensions
{
    public static AssertedTestCase CompleteTestCase(
        this DefinedTestCase testCase,
        ITestSuiteContextContainer contextContainer,
        params int[] testCasesToRun)
    {
        
        var operation = testCase.OperationFactory.Value;
        if (operation == null)
            throw new InvalidOperationException("Operation should be specified before completing test case. This is a unit test exception, ensure that test written correctly.");
        
        contextContainer.WithOperation(operation);
        
        var testCasesHash = new HashSet<int>(testCasesToRun);
        var testCasePipeline = new TestCasePipeline(testCasesHash);
        
        testCase.RegisterFutureValidation<OperationValidator>();
        testCase.RegisterFutureValidation<InputsValidator>();
        testCase.RegisterFutureValidation<ComparerValidator>();
        
        return testCasePipeline.ToCompleted(testCase);
    }
}