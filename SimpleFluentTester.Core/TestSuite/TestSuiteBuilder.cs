using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Suite
{
    public sealed class TestSuiteBuilder<TOutput>
    {
        private readonly TestSuiteBuilderContext<TOutput> _context;
    
        internal TestSuiteBuilder(TestSuiteBuilderContext<TOutput> context)
        {
            _context = context;
        }

        /// <summary>
        /// Specifies the expected value resulting from the execution of this test case.
        /// </summary>
        public TestCaseBuilder<TOutput> Expect(TOutput? expected)
        {
            return new TestCaseBuilder<TOutput>(_context, expected);
        }
    
        /// <summary>
        /// Specifies the method that needs to be tested.
        /// </summary>
        public TestSuiteBuilder<TOutput> UseOperation(Delegate operation)
        {
            _context.Operation.Value = operation;
            return this;
        }
    
        /// <summary>
        /// Allows defining a custom reporter that enables determining a custom report format.
        /// </summary>
        public TestSuiteBuilder<TOutput> WithCustomReporterFactory<TReporterFactory>()
            where TReporterFactory : ITestSuiteReporterFactory
        {
            _context.ReporterFactory = (TReporterFactory)Activator.CreateInstance(typeof(TReporterFactory));
            return this;
        }
    
        /// <summary>
        /// Specifies the name of the test suite run that will be shown in output.
        /// </summary>
        public TestSuiteBuilder<TOutput> WithDisplayName(string displayName)
        {
            _context.Name = displayName;
            return this;
        }
    
        /// <summary>
        /// Defines the return type of the function that we plan to test.
        /// The type should implement IEquatable interface or comparer should be provided. 
        /// </summary>
        public TestSuiteBuilder<TNewOutput> WithExpectedReturnType<TNewOutput>(Func<TNewOutput?, TNewOutput?, bool>? comparer = null)
        {
            var castedTestCases = _context.TestCases
                .Select(testCase =>
                {
                    if (testCase.Expected is not TNewOutput castedExpected)
                        throw new InvalidCastException("Expected type is not the same as operation type");
                    return new TestCase<TNewOutput>(testCase.Inputs, castedExpected, testCase.Number);
                })
                .ToList();

            var castedValidators = _context.Validators
                .Select(invoker => invoker as ValidationInvoker<TNewOutput>)
                .ToList();
            if (castedValidators == null || castedValidators.Any(x => x == null))
                throw new InvalidCastException("Couldn't cast validators, this should be a bug");
            var castedValidatorsHash = new HashSet<ValidationInvoker<TNewOutput>>(castedValidators);
        
            var newContext = new TestSuiteBuilderContext<TNewOutput>(
                _context.Number,
                _context.Name,
                _context.EntryAssemblyProvider,
                _context.Activator,
                castedTestCases,
                castedValidatorsHash,
                _context.ReporterFactory,
                _context.Operation,
                comparer,
                _context.ShouldBeExecuted);
            return new TestSuiteBuilder<TNewOutput>(newContext);
        }
    
        /// <summary>
        /// Add this call if you want your test suite to be ignored instead of commenting it, useful when you have multiple
        /// test cases in a single project.
        /// </summary>
        public TestSuiteBuilder<TOutput> Ignore
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
                    .Select(CompletedTestCase<TOutput>.NotExecuted)
                    .ToList();
                var unknownTestRunResult = new TestSuiteResult<TOutput>(testCases, 
                    new List<ValidationResult>(),
                    _context.Operation.Value?.Method, 
                    _context.Name,
                    _context.Number,
                    true);
                return (BaseTestRunReporter<TOutput>)_context.ReporterFactory.GetReporter(unknownTestRunResult);
            }
        
            _context.Operation.Value ??= TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(_context.EntryAssemblyProvider, _context.Activator);
        
            var testNumbersHash = new SortedSet<int>(testNumbers);

            if (!_context.IsObjectOutput)
                _context.RegisterValidator(typeof(OperationValidator), new OperationValidatedObject(typeof(TOutput)));
            _context.RegisterValidator(typeof(ComparerValidator), new EmptyValidatedObject());
            _context.RegisterValidator(typeof(TestNumbersValidator), new TestNumbersValidatedObject(testNumbersHash));

            var testCaseExecutor = new TestCaseExecutor<TOutput>(_context);
        
            var completedTestCases = _context.TestCases
                .Select(testCase => testCaseExecutor.TryCompeteTestCase(testCase, testNumbersHash))
                .ToList();
        
            var contextValidations = _context.Validators
                .Select(x => x.Invoke())
                .ToList();

            var testRunResult = new TestSuiteResult<TOutput>(completedTestCases, 
                contextValidations,
                _context.Operation.Value.Method,
                _context.Name,
                _context.Number);

            return (BaseTestRunReporter<TOutput>)_context.ReporterFactory.GetReporter(testRunResult);
        }
    }
}