using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite;

internal sealed class TestSuiteBuilder : ITestSuiteBuilder
{
    private readonly ITestSuiteContextContainer _contextContainer;
    
    internal TestSuiteBuilder(ITestSuiteContextContainer contextContainer)
    {
        _contextContainer = contextContainer;
    }

    private TestSuiteBuilder(TestSuiteBuilder builder)
    {
        _contextContainer = builder._contextContainer;
    }

    /// <summary>
    /// Specifies the expected value resulting from the execution of this test case.
    /// </summary>
    public ITestCaseBuilder Expect(object? expected)
    {
        var comparedObj = ComparedObjectFactory.Wrap(expected);
        return new TestCaseBuilder(_contextContainer, comparedObj);
    }
    
    /// <summary>
    /// Specifies the expected Exception thrown from the execution of this test case.
    /// </summary>
    public ITestCaseBuilder ExpectException<TException>(string? message = null)
        where TException : Exception
    {
        var validatedException = ExpectExceptionFactory.Create(_contextContainer, typeof(TException), message);
        return new TestCaseBuilder(_contextContainer, validatedException.Object, validatedException.ValidationResult);
    }
    
    /// <summary>
    /// Specifies the method that needs to be tested.
    /// </summary>
    public ITestSuiteBuilder UseOperation(Delegate operation)
    {
        _contextContainer.WithOperation(operation);
        return new TestSuiteBuilder(this);
    }
    
    /// <summary>
    /// Specifies the name of the test suite run that will be shown in an output.
    /// </summary>
    public ITestSuiteBuilder WithDisplayName(string displayName)
    {
        _contextContainer.WithDisplayName(displayName);
        return new TestSuiteBuilder(this);
    }

    /// <summary>
    /// Defines the return type of the function that we plan to test.
    /// The type should implement IEquatable interface or comparer should be provided. 
    /// </summary>
    public ITestSuiteBuilder WithComparer<TExpected>(Func<TExpected?, TExpected?, bool> comparer)
    {
        _contextContainer.WithComparer(comparer);
        return new TestSuiteBuilder(this);
    }
    
    /// <summary>
    /// Add this call if you want your test suite to be ignored instead of commenting it, useful when you have multiple
    /// test cases in a single project.
    /// </summary>
    public ITestSuiteBuilder Ignore
    {
        get
        {
            _contextContainer.DoNotExecute();
            return new TestSuiteBuilder(this);
        }
    }

    /// <summary>
    /// Initiates the execution of test cases defined earlier using <see cref="Expect"/>.
    /// For debugging failed test cases, it also allows selecting the test case numbers that should be executed, all others will be skipped.
    /// </summary>
    public ITestSuiteReporter Run(params int[] testNumbers)
    {
        if (!_contextContainer.Context.ShouldBeExecuted)
            return ReturnNotExecutedTestReporter(_contextContainer.Context);

        var testSuiteResult = ProcessContextToResult(testNumbers);
        
        return new TestSuiteReporter(testSuiteResult);
    }
    
    private static ITestSuiteReporter ReturnNotExecutedTestReporter(ITestSuiteContext context)
    {
        var testCases = context.TestCases
            .Select(CompletedTestCase.NotExecuted)
            .ToList();
        
        var testSuiteRunResult = GetTestSuiteRunResult(context, testCases, PackedValidation.Empty);
        
        return new TestSuiteReporter(testSuiteRunResult);
    }

    private TestSuiteRunResult ProcessContextToResult(IEnumerable<int> testNumbers)
    {
        var testNumbersHash = new SortedSet<int>(testNumbers);
        
        _contextContainer.TryToEnrichAttributeOperation();
        
        var contextValidation = ValidateContext(testNumbersHash);
        
        var completedTestCases = ExecuteTestCases(testNumbersHash);
        
        return GetTestSuiteRunResult(_contextContainer.Context, completedTestCases, contextValidation);
    }

    private PackedValidation ValidateContext(ISet<int> testNumbersHash)
    {
        _contextContainer.Context.RegisterValidation<ComparerValidator>();
        _contextContainer.Context.RegisterValidation<TestNumbersValidator>(() => new TestNumbersValidationContext(testNumbersHash));
        return ValidationPipe.ValidatePacked(_contextContainer.Context);
    }

    private IList<CompletedTestCase> ExecuteTestCases(ISet<int> testNumbersHash)
    {
        var testCasePipeline = new TestCasePipeline(
            testNumbersHash);
        return _contextContainer.Context.TestCases
            .Select(testCase => testCasePipeline.ToCompleted(testCase))
            .ToList();
    }
    
    private static TestSuiteRunResult GetTestSuiteRunResult(
        ITestSuiteContext context,
        IList<CompletedTestCase> completedTestCases,
        PackedValidation contextValidation)
    {
        return new TestSuiteRunResult(completedTestCases,
            contextValidation,
            context.Operation,
            context.Name,
            context.Number,
            context.ShouldBeExecuted);
    }
}