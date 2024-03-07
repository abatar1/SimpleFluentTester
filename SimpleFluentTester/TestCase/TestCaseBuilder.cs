using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase;

internal sealed class TestCaseBuilder<TOutput>(
    ITestSuiteBuilderContext<TOutput> context, 
    TOutput? expected) : ITestCaseBuilder<TOutput>
{
    /// <summary>
    /// Defines input parameters; their types should match the types of the tested method's input parameters and should be in the same order.
    /// </summary>
    public ITestSuiteBuilder<TOutput> WithInput(params object?[] inputs)
    {
        var testCase = new TestCase<TOutput>(inputs, expected, context.TestCases.Count + 1);
        
        testCase.RegisterValidator(typeof(OperationValidator), new OperationValidatedObject(expected?.GetType()));
        testCase.RegisterValidator(typeof(InputsValidator), new InputsValidatedObject(inputs));
            
        context.TestCases.Add(testCase);

        return new TestSuiteBuilder<TOutput>(context);
    }
}