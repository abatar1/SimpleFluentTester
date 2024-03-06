using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
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
        _context.Operation.Value = operation;
        return this;
    }
    
    /// <summary>
    /// Specifies the name of the test suite run that will be shown in output.
    /// </summary>
    public ITestSuiteBuilder<TOutput> WithDisplayName(string displayName)
    {
        _context.Name = displayName;
        return this;
    }
    
    /// <summary>
    /// Defines the return type of the function that we plan to test.
    /// The type should implement IEquatable interface or comparer should be provided. 
    /// </summary>
    public ITestSuiteBuilder<TNewOutput> WithComparer<TNewOutput>(Func<TNewOutput?, TNewOutput?, bool>? comparer = null)
    {
        var castedTestCases = _context.TestCases
            .Select(testCase =>
            {
                if (testCase.Expected is not TNewOutput castedExpected)
                    throw new InvalidCastException("Expected type is not the same as operation type");
                return new TestCase<TNewOutput>(testCase.Inputs, castedExpected, testCase.Number);
            })
            .ToList();
        
        var newContext = new TestSuiteBuilderContext<TNewOutput>(
            _context.Number,
            _context.Name,
            _context.EntryAssemblyProvider,
            _context.Activator,
            castedTestCases,
            _context.Operation,
            comparer,
            _context.ShouldBeExecuted);
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
            _context.ShouldBeExecuted = false;
            return this;
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
        
        _context.Operation.Value ??= TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(_context.EntryAssemblyProvider, _context.Activator);
        
        var testNumbersHash = new SortedSet<int>(testNumbers);
        
        var testCaseExecutor = new TestCaseExecutor<TOutput>(_context);
        var completedTestCases = _context.TestCases
            .Select(testCase => testCaseExecutor.TryCompeteTestCase(testCase, testNumbersHash))
            .ToList();

        var contextValidationResult = InvokeContextValidators(_context, testNumbersHash);

        var testRunResult = new TestSuiteResult<TOutput>(completedTestCases, 
            contextValidationResult,
            _context.Operation.Value.Method,
            _context.Name,
            _context.Number);
        return new TestSuiteReporter<TOutput>(testRunResult);
    }

    private static ITestSuiteReporter<TOutput> ReturnNotExecutedTestReporter(ITestSuiteBuilderContext<TOutput> context)
    {
        var testCases = context.TestCases
            .Select(CompletedTestCase<TOutput>.NotExecuted)
            .ToList();
        var unknownTestRunResult = new TestSuiteResult<TOutput>(testCases, 
            new List<ValidationResult>(),
            context.Operation.Value?.Method, 
            context.Name,
            context.Number,
            true);
        return new TestSuiteReporter<TOutput>(unknownTestRunResult);
    }

    private static IList<ValidationResult> InvokeContextValidators(ITestSuiteBuilderContext<TOutput> context, SortedSet<int> testNumbersHash)
    {
        var contextValidationResult = new List<ValidationResult>();
        if (!context.IsObjectOutput)
            contextValidationResult.Add(context.InvokeValidation(typeof(OperationValidator), new OperationValidatedObject(typeof(TOutput))));
        contextValidationResult.Add(context.InvokeValidation(typeof(ComparerValidator), new EmptyValidatedObject()));
        contextValidationResult.Add(context.InvokeValidation(typeof(TestNumbersValidator), new TestNumbersValidatedObject(testNumbersHash)));
        return contextValidationResult;
    } 
}