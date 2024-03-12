using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests;

public static class TestHelpers
{
    public static ITestSuiteBuilderContext<TOutput> CreateEmptyContext<TOutput>(
        IEntryAssemblyProvider? assemblyProvider = null,
        IActivator? activator = null,
        Delegate? operation = null)
    {
        return new TestSuiteBuilderContext<TOutput>(
            0,
            nameof(TestHelpers),
            assemblyProvider ?? new EntryAssemblyProvider(),
            activator ?? new DefaultActivator(),
            new List<TestCase<TOutput>>(),
            operation,
            null,
            new Dictionary<ValidationSubject, IList<ValidationResult>>(),
            true);
    }

    public static TestSuiteResult<TOutput> GetTestSuiteResult<TOutput>(
        ValidationResult validationResult,
        TestCaseOperation<TOutput> testCaseOperation,
        int testCaseToRun = 1,
        bool ignored = false)
    {
        var validationResults = new List<ValidationResult> { validationResult };
        var context = CreateEmptyContext<TOutput>();
        var completedTestCase = CompleteTestCase(context, testCaseOperation, testCaseToRun);

        return new TestSuiteResult<TOutput>(
            new List<CompletedTestCase<TOutput>> { completedTestCase },
            validationResults,
            context.Operation,
            context.Name,
            context.Number,
            GetValidationStatus(validationResult, ignored));
    }

    public static CompletedTestCase<TOutput> CompleteTestCase<TOutput>(
        ITestSuiteBuilderContext<TOutput> context,
        TestCaseOperation<TOutput> testCaseOperation,
        int testCaseToRun = 1)
    {
        var newContext = context.WithOperation(testCaseOperation.Operation);
        var testCaseExecutor = new TestCaseExecutor<TOutput>(newContext);
        var testCase = testCaseOperation.TestCase;
        testCase.RegisterValidator(typeof(OperationValidator),
            new OperationValidatedObject(testCase.Expected?.GetType()));
        testCase.RegisterValidator(typeof(InputsValidator), new InputsValidatedObject(testCase.Inputs));
        return testCaseExecutor.TryCompeteTestCase(testCase, new SortedSet<int>([testCaseToRun]));
    }
    
    public static class ValidationResults
    {
        public static ValidationResult Valid => new(ValidationStatus.Valid, ValidationSubject.Operation);
        
        public static ValidationResult NonValid => new(ValidationStatus.NonValid, ValidationSubject.Operation);
    }

    public static class TestCaseOperations
    {
        private static int Operation(int x, int y) => x + y;
        
        private static int ThrowOperation(int _, int __) => throw new Exception();
        
        public static TestCaseOperation<int> Passed
        {
            get
            {
                var testCase = new TestCase<int>([1, 2], 3, 1);
                return new TestCaseOperation<int>(Operation, testCase);
            }
        }
        
        public static TestCaseOperation<int> NotPassed
        {
            get
            {
                var testCase = new TestCase<int>([1, 2], 4, 1);
                return new TestCaseOperation<int>(Operation, testCase);
            }
        }
        
        public static TestCaseOperation<int> NotPassedWithException
        {
            get
            {
                var testCase = new TestCase<int>([1, 2], 3, 1);
                return new TestCaseOperation<int>(ThrowOperation, testCase);
            }
        }
        
        public static TestCaseOperation<int> Invalid
        {
            get
            {
                var testCase = new TestCase<int>(["test", 2], 4, 1);
                return new TestCaseOperation<int>(Operation, testCase);
            }
        }
    }

    private static ValidationStatus GetValidationStatus(ValidationResult validationResult, bool ignored = false)
    {
        if (ignored)
            return ValidationStatus.Ignored;

        if (validationResult.IsValid)
            return ValidationStatus.Valid;

        return ValidationStatus.NonValid;
    }
    
    public sealed class TestCaseOperation<TOutput>(Delegate operation, TestCase<TOutput> testCase)
    {
        public Delegate Operation { get; } = operation;

        public TestCase<TOutput> TestCase { get; } = testCase;
    }

    internal static int Adder(int number1, int number2)
    {
        return number1 + number2;
    }
}

internal class CustomException : Exception;