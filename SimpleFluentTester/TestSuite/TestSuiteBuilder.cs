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
    private readonly IComparedObjectFactory _comparedObjectFactory;
    private readonly IValidationUnpacker _validationUnpacker;
    
    internal TestSuiteBuilder(ITestSuiteContextContainer contextContainer)
    {
        _contextContainer = contextContainer;
        _comparedObjectFactory = new ComparedObjectFactory();
        _validationUnpacker = new ValidationUnpacker();
    }

    private TestSuiteBuilder(TestSuiteBuilder builder)
    {
        _contextContainer = builder._contextContainer;
        _comparedObjectFactory = builder._comparedObjectFactory;
        _validationUnpacker = builder._validationUnpacker;
    }

    /// <summary>
    /// Specifies the expected value resulting from the execution of this test case.
    /// </summary>
    public ITestCaseBuilder Expect(object? expected)
    {
        var comparedObj = _comparedObjectFactory.Wrap(expected);
        return new TestCaseBuilder(_contextContainer, _comparedObjectFactory, comparedObj);
    }
    
    /// <summary>
    /// Specifies the expected Exception thrown from the execution of this test case.
    /// </summary>
    public ITestCaseBuilder ExpectException<TException>(string? message = null)
        where TException : Exception
    {
        var validatedException = ExpectExceptionFactory.Create(_contextContainer, _comparedObjectFactory, typeof(TException), message);
        return new TestCaseBuilder(_contextContainer, _comparedObjectFactory, validatedException.Object, validatedException.ValidationResult);
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
        
        var testSuiteResult = GetTestSuiteResult(context, testCases, ValidationUnpacked.Empty);
        
        return new TestSuiteReporter(testSuiteResult);
    }

    private TestSuiteResult ProcessContextToResult(IEnumerable<int> testNumbers)
    {
        var testNumbersHash = new SortedSet<int>(testNumbers);
        
        _contextContainer.TryToEnrichAttributeOperation();
        
        var contextValidationResults = ValidateContext(testNumbersHash);
        
        var completedTestCases = ExecuteTestCases(testNumbersHash);
        
        return GetTestSuiteResult(_contextContainer.Context, completedTestCases, contextValidationResults);
    }

    private ValidationUnpacked ValidateContext(ISet<int> testNumbersHash)
    {
        _contextContainer.Context.RegisterValidation<ComparerValidator>();
        _contextContainer.Context.RegisterValidation<TestNumbersValidator>(() => new TestNumbersValidatedObject(testNumbersHash));
        return _validationUnpacker.Unpack(_contextContainer.Context);
    }

    private IList<CompletedTestCase> ExecuteTestCases(ISet<int> testNumbersHash)
    {
        var testCasePipeline = new TestCasePipeline(_contextContainer.Context, _comparedObjectFactory,
            _validationUnpacker, testNumbersHash);
        return _contextContainer.Context.TestCases
            .Select(testCase => testCasePipeline.ToCompleted(testCase))
            .ToList();
    }
    
    private static TestSuiteResult GetTestSuiteResult(
        ITestSuiteContext context,
        IList<CompletedTestCase> completedTestCases,
        ValidationUnpacked validationUnpacked)
    {
        return new TestSuiteResult(completedTestCases,
            validationUnpacked,
            context.Operation,
            context.Name,
            context.Number,
            context.ShouldBeExecuted);
    }
}