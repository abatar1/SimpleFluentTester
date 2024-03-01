using System;
using System.Collections.Generic;
using System.Linq;
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
        
        var testNumbersHash = new HashSet<int>(testNumbers);

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

    private ValidatedTestCase<TOutput> TryToValidateTestCase(TestCase<TOutput> testCase, HashSet<int> testNumbersHash)
    {
        var shouldBeExecuted = testNumbersHash.Count == 0 ||
                               (testNumbersHash.Count != 0 && testNumbersHash.Contains(testCase.Number));
        if (!shouldBeExecuted)
            return new ValidatedTestCase<TOutput>(ValidationStatus.Unknown, testCase, new List<ValidationResult>());
                        
        var validationResults = testCase.Validators
            .Select(x => x.Invoke())
            .ToList();

        var isValid = validationResults.All(x => x.IsValid);
        if (isValid)
            _ = testCase.Assert.Value;
        var validationStatus = isValid switch
        {
            true => ValidationStatus.Valid,
            false => ValidationStatus.NonValid
        };
        return new ValidatedTestCase<TOutput>(validationStatus, testCase, validationResults);
    }
}