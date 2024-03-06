using SimpleFluentTester.Suite;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase
{
    public sealed class TestCaseBuilder<TOutput>(
        TestSuiteBuilderContext<TOutput> context, 
        TOutput? expected)
    {
        /// <summary>
        /// Defines input parameters; their types should match the types of the tested method's input parameters and should be in the same order.
        /// </summary>
        public TestSuiteBuilder<TOutput> WithInput(params object?[] inputs)
        {
            var testCase = new TestCase<TOutput>(inputs, expected, context.TestCases.Count + 1);
        
            if (context.IsObjectOutput)
                testCase.RegisterValidator(context, typeof(OperationValidator), new OperationValidatedObject(expected?.GetType()));
            testCase.RegisterValidator(context, typeof(InputsValidator), new InputsValidatedObject(inputs));
            
            context.TestCases.Add(testCase);

            return new TestSuiteBuilder<TOutput>(context);
        }
    }
}