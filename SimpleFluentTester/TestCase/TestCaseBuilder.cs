using System;
using System.Collections.Generic;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase;

/// <summary>
/// Provides functionality to build test cases within a specific testing context.
/// Encapsulates test clauses, validation results, and operations for managing and validating test cases.
/// </summary>
internal sealed class TestCaseBuilder(
    ITestSuiteContextContainer contextContainer,
    List<DefinedTestClause> testClauses,
    IList<ValidationResult>? validationResults = null) : ITestCaseBuilder
{
    public ITestSuiteBuilder WithInput(params object?[] inputs)
    {
        var testCase = new DefinedTestCase(
            new Lazy<Delegate?>(() => contextContainer.Context.Operation),
            new Lazy<Delegate?>(() => contextContainer.Context.Comparer), 
            ComparedObjectFactory.WrapMany(inputs), 
            testClauses, 
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

        return new SequentialTestSuiteBuilder(contextContainer, new List<DefinedTestClause>());
    }

    public ITestSuiteBuilder And => new SequentialTestSuiteBuilder(contextContainer, testClauses);
}