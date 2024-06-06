using System.Collections.Generic;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

internal sealed class TestCaseBuilder(
    ITestSuiteContextContainer contextContainer, 
    IComparedObject expected, 
    IList<ValidationResult>? validationResults = null) : ITestCaseBuilder
{
    /// <summary>
    /// Defines input parameters; their types should match the types of the tested method's input parameters and should be in the same order.
    /// </summary>
    public ITestSuiteBuilder WithInput(params object?[] inputs)
    {
        var testCase = new TestCase(
            () => contextContainer.Context.Operation,
            () => contextContainer.Context.Comparer, 
            ComparedObjectFactory.WrapMany(inputs), 
            expected, 
            contextContainer.Context.TestCases.Count + 1);

        if (validationResults != null && validationResults.Count != 0)
        {
            foreach (var validationResult in validationResults)
                testCase.AddValidation(validationResult);
        }
        
        testCase.RegisterValidation<OperationValidator>(() => new OperationValidationContext(contextContainer.Context.Operation));
        testCase.RegisterValidation<InputsValidator>(() => new InputsValidationContext(contextContainer.Context.Operation));
            
        contextContainer.Context.TestCases.Add(testCase);

        return new TestSuiteBuilder(contextContainer);
    }
}