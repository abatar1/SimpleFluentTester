using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite;

internal sealed class TestSuiteBuilder<TOutput> : ITestSuiteBuilder<TOutput>
{
    private readonly ITestSuiteBuilderContext<TOutput> _context;
    
    internal TestSuiteBuilder(ITestSuiteBuilderContext<TOutput> context)
    {
        _context = context;
    }

    /// <summary>
    /// Specifies the expected value resulting from the execution of this test case.
    /// </summary>
    public ITestCaseBuilder<TOutput> Expect(TOutput? expected)
    {
        return new TestCaseBuilder<TOutput>(_context, expected);
    }
    
    /// <summary>
    /// Specifies the method that needs to be tested.
    /// </summary>
    public ITestSuiteBuilder<TOutput> UseOperation(Delegate operation)
    {
        var newContext = _context.WithOperation(operation);
        return new TestSuiteBuilder<TOutput>(newContext);
    }
    
    /// <summary>
    /// Specifies the name of the test suite run that will be shown in an output.
    /// </summary>
    public ITestSuiteBuilder<TOutput> WithDisplayName(string displayName)
    {
        var newContext = _context.WithDisplayName(displayName);
        return new TestSuiteBuilder<TOutput>(newContext);
    }
    
    /// <summary>
    /// Defines the return type of the function that we plan to test.
    /// The type should implement IEquatable interface or comparer should be provided. 
    /// </summary>
    public ITestSuiteBuilder<TNewOutput> WithComparer<TNewOutput>(Func<TNewOutput?, TNewOutput?, bool>? comparer = null)
    {
        ITestSuiteBuilderContext<TNewOutput> newContext;
        try
        {
            var castedTestCases = _context.TestCases
                .Select(testCase =>
                {
                    if (testCase.Expected is not TNewOutput castedExpected)
                        throw new InvalidCastException($"Expected type {testCase.Expected?.GetType()} is not the same as operation type {typeof(TNewOutput)}");
                    return new TestCase<TNewOutput>(testCase.Inputs, castedExpected, testCase.Number);
                })
                .ToList();
            newContext = _context.ConvertType(castedTestCases, comparer);
        }
        catch (Exception e)
        {
            newContext = _context.ConvertType(new List<TestCase<TNewOutput>>(), comparer);
            newContext.AddValidation(ValidationResult.Failed(ValidationSubject.Comparer, e.Message));
        }
        
        return new TestSuiteBuilder<TNewOutput>(newContext);
    }
    
    /// <summary>
    /// Add this call if you want your test suite to be ignored instead of commenting it, useful when you have multiple
    /// test cases in a single project.
    /// </summary>
    public ITestSuiteBuilder<TOutput> Ignore
    {
        get
        {
            var newContext = _context.DoNotExecute();
            return new TestSuiteBuilder<TOutput>(newContext);
        }
    }

    /// <summary>
    /// Initiates the execution of test cases defined earlier using <see cref="Expect"/>.
    /// For debugging failed test cases, it also allows selecting the test case numbers that should be executed, all others will be skipped.
    /// </summary>
    public ITestSuiteReporter<TOutput> Run(params int[] testNumbers)
    {
        if (!_context.ShouldBeExecuted)
            return ReturnNotExecutedTestReporter(_context);

        var testSuiteResult = ProcessContextToResult(_context, testNumbers);
        return new TestSuiteReporter<TOutput>(testSuiteResult);
    }

    private static TestSuiteResult<TOutput> ProcessContextToResult(
        ITestSuiteBuilderContext<TOutput> context,
        IEnumerable<int> testNumbers)
    {
        var testNumbersHash = new SortedSet<int>(testNumbers);
        
        // Enrich section
        var enrichedContext = context.TryToEnrichAttributeOperation();

        // Validation section
        enrichedContext.InvokeValidation(typeof(OperationValidator), new OperationValidatedObject(typeof(TOutput)));
        enrichedContext.InvokeValidation(typeof(ComparerValidator), new EmptyValidatedObject());
        enrichedContext.InvokeValidation(typeof(TestNumbersValidator), new TestNumbersValidatedObject(testNumbersHash));
        
        // Execute section
        var testCaseExecutor = new TestCaseExecutor<TOutput>(enrichedContext);
        var completedTestCases = enrichedContext.TestCases
            .Select(testCase => testCaseExecutor.TryCompeteTestCase(testCase, testNumbersHash))
            .ToList();
        
        return GetTestSuiteResult(enrichedContext, completedTestCases);
    }

    private static ITestSuiteReporter<TOutput> ReturnNotExecutedTestReporter(ITestSuiteBuilderContext<TOutput> context)
    {
        var testCases = context.TestCases
            .Select(CompletedTestCase<TOutput>.NotExecuted)
            .ToList();
        var testSuiteResult = GetTestSuiteResult(context, testCases);
        return new TestSuiteReporter<TOutput>(testSuiteResult);
    }
    
    private static TestSuiteResult<TOutput> GetTestSuiteResult(
        ITestSuiteBuilderContext<TOutput> context,
        IList<CompletedTestCase<TOutput>> completedTestCases)
    {
        List<ValidationResult> validations;
        var validationStatus = ValidationStatus.Valid;
        if (!context.ShouldBeExecuted)
        {
            validations = [];
            validationStatus = ValidationStatus.Ignored;
        }
        else
        {
            validations = context.Validations
                .Select(x =>
                {
                    var message = string.Join("\n", x.Value
                        .Where(y => !string.IsNullOrWhiteSpace(y.Message)));
                    var valid = x.Value
                        .All(y => y.IsValid);
                    if (!valid)
                        validationStatus = ValidationStatus.NonValid;
                    
                    return new ValidationResult(validationStatus, x.Key, message);
                })
                .ToList();
        }
      
        return new TestSuiteResult<TOutput>(completedTestCases,
            validations,
            context.Operation,
            context.Name,
            context.Number,
            validationStatus);
    }
}