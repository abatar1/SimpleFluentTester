using System;
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
    public ITestSuiteBuilder WithInput(params object?[] inputs)
    {
        var testCase = new DeferredTestCase(
            new Lazy<Delegate?>(() => contextContainer.Context.Operation),
            new Lazy<Delegate?>(() => contextContainer.Context.Comparer), 
            ComparedObjectFactory.WrapMany(inputs), 
            expected, 
            contextContainer.Context.TestCases.Count + 1);

        if (validationResults != null && validationResults.Count != 0)
        {
            foreach (var validationResult in validationResults)
                testCase.AddReadyValidation(validationResult);
        }
        
        testCase.RegisterFutureValidation<OperationValidator>();
        testCase.RegisterFutureValidation<InputsValidator>();
        testCase.RegisterFutureValidation<ComparerValidator>();
            
        contextContainer.Context.TestCases.Add(testCase);

        return new TestSuiteBuilder(contextContainer);
    }

    public ITestSuiteBuilder And
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }
}