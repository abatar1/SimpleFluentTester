using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestRun;

public sealed class TestRunBuilder<TOutput>
{
    private readonly TestRunBuilderContext<TOutput> _context;
    
    internal TestRunBuilder(TestRunBuilderContext<TOutput> context)
    {
        _context = context;
    }

    /// <summary>
    /// Specifies the expected value resulting from the execution of this test case.
    /// </summary>
    public TestCaseBuilder<TOutput> Expect(TOutput? expected)
    {
        return new TestCaseBuilder<TOutput>(expected, _context);
    }
    
    /// <summary>
    /// Specifies the method that needs to be tested.
    /// </summary>
    public TestRunBuilder<TOutput> UseOperation(Delegate operation)
    {
        _context.Operation.Value = operation;
        return this;
    }
    
    /// <summary>
    /// Allows defining a custom reporter that enables determining a custom report format.
    /// </summary>
    public TestRunBuilder<TOutput> WithCustomReporterFactory<TReporterFactory>()
        where TReporterFactory : ITestRunReporterFactory
    {
        _context.ReporterFactory = (TReporterFactory)Activator.CreateInstance(typeof(TReporterFactory));
        return this;
    }
    
    /// <summary>
    /// Defines the return type of the function that we plan to test.
    /// The type should implement IEquatable interface or comparer should be provided. 
    /// </summary>
    public TestRunBuilder<TNewOutput> WithExpectedReturnType<TNewOutput>(Func<TNewOutput?, TNewOutput?, bool>? comparer = null)
    {
        var castedTestCases = _context.TestCases
            .Select(testCase =>
            {
                if (testCase.Expected is not TNewOutput castedExpected)
                    throw new InvalidCastException("Expected type is not the same as operation type");
                var conversionExpression = Expression.Convert(testCase.Assert.AssertExpression.Body, typeof(TNewOutput));
                var castedAssertExpression = Expression.Lambda<Func<Assert<TNewOutput>>>(conversionExpression,
                    testCase.Assert.AssertExpression.Parameters);
                var lazyAssert = new LazyAssert<TNewOutput>(castedAssertExpression);
                
                return new TestCase<TNewOutput>(testCase.Inputs, castedExpected, lazyAssert, testCase.Number);
            })
            .ToList();

        var castedValidators = _context.Validators
            .Select(x => x as ValidationInvoker<TNewOutput>)
            .ToList();
        if (castedValidators == null || castedValidators.Any(x => x == null))
            throw new InvalidCastException("Couldn't cast validators, this should be a bug");
        var castedValidatorsHash = new HashSet<ValidationInvoker<TNewOutput>>(castedValidators);
        
        var newContext = new TestRunBuilderContext<TNewOutput>(
            _context.EntryAssemblyProvider,
            _context.Activator,
            castedTestCases,
            castedValidatorsHash,
            _context.ReporterFactory,
            _context.Operation,
            comparer,
            _context.ShouldBeExecuted);
        return new TestRunBuilder<TNewOutput>(newContext);
    }
    
    /// <summary>
    /// Add this call if you want your test suite to be ignored instead of commenting it, useful when you have multiple
    /// test cases in a single project.
    /// </summary>
    public TestRunBuilder<TOutput> Ignore
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
    public BaseTestRunReporter<TOutput> Run(params int[] testNumbers)
    {
        if (!_context.ShouldBeExecuted)
        {
            var testCases = _context.TestCases
                .Select(testCase =>
                    new ValidatedTestCase<TOutput>(ValidationStatus.Unknown, testCase, new List<ValidationResult>()))
                .ToList();
            var unknownTestRunResult = new TestRunResult<TOutput>(testCases, 
                new List<ValidationResult>(),
                _context.Operation.Value?.Method, 
                true);
            return (BaseTestRunReporter<TOutput>)_context.ReporterFactory.GetReporter(unknownTestRunResult);
        }
        
        _context.Operation.Value ??= TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(_context.EntryAssemblyProvider, _context.Activator);
        
        var testNumbersHash = new SortedSet<int>(testNumbers);

        if (!_context.IsObjectOutput)
            _context.RegisterValidator(typeof(OperationValidator), new OperationValidatedObject(typeof(TOutput)));
        _context.RegisterValidator(typeof(ComparerValidator), new EmptyValidatedObject());
        _context.RegisterValidator(typeof(TestNumbersValidator), new TestNumbersValidatedObject(testNumbersHash));
        
        var testCasesWithValidation = _context.TestCases
            .Select(testCase => TryToValidateTestCase(testCase, testNumbersHash))
            .ToList();
        
        var contextValidations = _context.Validators
            .Select(x => x.Invoke())
            .ToList();

        var testRunResult = new TestRunResult<TOutput>(testCasesWithValidation, 
            contextValidations,
            _context.Operation.Value.Method);

        return (BaseTestRunReporter<TOutput>)_context.ReporterFactory.GetReporter(testRunResult);
    }

    private ValidatedTestCase<TOutput> TryToValidateTestCase(TestCase<TOutput> testCase, SortedSet<int> testNumbersHash)
    {
        if (!ShouldBeExecuted(testCase, testNumbersHash))
            return new ValidatedTestCase<TOutput>(ValidationStatus.Unknown, testCase, new List<ValidationResult>());
                        
        var validationResults = testCase.Validators
            .Select(x => x.Invoke())
            .ToList();

        var nonValid = validationResults.Any(x => !x.IsValid);
        if (!nonValid)
            _ = testCase.Assert.Value;
        var validationStatus = nonValid switch
        {
            true => ValidationStatus.NonValid,
            false => ValidationStatus.Valid
        };
        return new ValidatedTestCase<TOutput>(validationStatus, testCase, validationResults);
    }

    private bool ShouldBeExecuted(TestCase<TOutput> testCase, SortedSet<int> testNumbersHash)
    {
        if (testNumbersHash.Count == 0)
            return true;
        if (testNumbersHash.Count != 0)
            return testNumbersHash.Max > _context.TestCases.Count || testNumbersHash.Contains(testCase.Number);

        throw new Exception("This code shouldn't be reached.");
    }
}