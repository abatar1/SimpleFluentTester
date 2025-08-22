using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestCase.Pipeline;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.TestSuite.Parameter;

namespace SimpleFluentTester.TestSuite;

internal sealed class SequentialTestSuiteBuilder : ITestSuiteBuilder
{
    private readonly ITestSuiteContextContainer _contextContainer;
    private readonly List<DefinedTestClause> _testClauses;
    
    internal SequentialTestSuiteBuilder(ITestSuiteContextContainer contextContainer, List<DefinedTestClause> testClauses)
    {
        _contextContainer = contextContainer;
        _testClauses = testClauses;
    }

    private SequentialTestSuiteBuilder(SequentialTestSuiteBuilder builder)
    {
        _contextContainer = builder._contextContainer;
        _testClauses = new List<DefinedTestClause>();
    }
    
    public ITestCaseBuilder ExpectReturn(object? expected)
    {
        var comparedObj = ComparedObjectFactory.Wrap(expected);
        
        var testClause = new DefinedTestClause(comparedObj);
        _testClauses.Add(testClause);
        
        return new TestCaseBuilder(_contextContainer, _testClauses);
    }
    
    public ITestCaseBuilder ExpectException<TException>(string? message = null)
        where TException : Exception
    {
        var validatedException = ExpectExceptionFactory.Create(_contextContainer, typeof(TException), message);
        
        var testClause = new DefinedTestClause(validatedException.Object);
        _testClauses.Add(testClause);
        
        return new TestCaseBuilder(_contextContainer, _testClauses, validatedException.ValidationResult);
    }

    public IParameterExpectationBuilder ExpectParameter(string parameterName)
    {
        var validatedParameter = ExpectParameterFactory.Create(_contextContainer, parameterName);
        
        return new ParameterExpectationBuilder(_contextContainer, validatedParameter, _testClauses);
    }

    public IParameterExpectationBuilder ExpectParameter(int parameterPosition)
    {
        var validatedParameter = ExpectParameterFactory.Create(_contextContainer, parameterPosition);
        return new ParameterExpectationBuilder(_contextContainer, validatedParameter, _testClauses);
    }

    public ITestSuiteBuilder UseOperation(Delegate operation)
    {
        _contextContainer.WithOperation(operation);
        return new SequentialTestSuiteBuilder(this);
    }
    
    public ITestSuiteBuilder WithDisplayName(string displayName)
    {
        _contextContainer.WithDisplayName(displayName);
        return new SequentialTestSuiteBuilder(this);
    }
    
    public ITestSuiteBuilder WithComparer<TExpected>(ComparerDelegate<TExpected> comparer)
    {
        _contextContainer.WithComparer(comparer);
        return new SequentialTestSuiteBuilder(this);
    }

    public ITestSuiteBuilder Ignore
    {
        get
        {
            _contextContainer.DoNotExecute();
            return new SequentialTestSuiteBuilder(this);
        }
    }
    
    public ITestSuiteReporter Run(params int[] testNumbers)
    {
        if (!CheckIfShouldBeExecuted())
            return ReturnNotExecutedTestReporter();

        var testSuiteResult = ProcessContextToResult(testNumbers);
        return new TestSuiteReporter(testSuiteResult);
    }

    /// <summary>
    /// Determines whether the test suite should be executed based on the current context and global state.
    /// </summary>
    /// <returns>True if the test suite should be executed; otherwise, false.</returns>
    private bool CheckIfShouldBeExecuted()
    {
        return _contextContainer.Context.ShouldBeExecuted && TestSuiteGlobalState.IsAllowed(_contextContainer.Context);
    }

    /// <summary>
    /// Processes the current test suite context and generates a result based on the given test numbers.
    /// </summary>
    /// <param name="testNumbers">The collection of test numbers to be executed within the context.</param>
    /// <returns>An instance of <see cref="TestSuiteRunResult"/> containing the results of the executed test cases and validation.</returns>
    private TestSuiteRunResult ProcessContextToResult(IEnumerable<int> testNumbers)
    {
        var testNumbersHash = new SortedSet<int>(testNumbers);
        ValidateContext(testNumbersHash);
        
        var operationEnricher = new OperationEnricher(_contextContainer);
        operationEnricher.TryToEnrichAttributeOperation();
        
        var completedTestCases = ExecuteTestCases(testNumbersHash);
        
        return GetTestSuiteRunResult(_contextContainer.Context, completedTestCases, true);
    }
    
    /// <summary>
    /// Creates a reporter for a test suite that was not executed.
    /// </summary>
    /// <returns>An instance of <see cref="ITestSuiteReporter"/> representing a not-executed test suite.</returns>
    private ITestSuiteReporter ReturnNotExecutedTestReporter(Exception? exception = null)
    {
        var testCases = _contextContainer.Context.TestCases
            .Select(ITestCase (x) => x.AsIgnored().AsIgnored())
            .ToList();
        
        var testSuiteRunResult = GetTestSuiteRunResult(_contextContainer.Context, testCases, false, exception);
        
        return new TestSuiteReporter(testSuiteRunResult);
    }

    /// <summary>
    /// Validates the current test context and checks for any invalid states or criteria based on the provided test numbers.
    /// </summary>
    /// <param name="testNumbersHash">A set of test case indices to validate against the current test suite context.</param>
    private void ValidateContext(ISet<int> testNumbersHash)
    {
        if (testNumbersHash.Count != 0 && (testNumbersHash.Count > _contextContainer.Context.TestCases.Count || testNumbersHash.Max() > _contextContainer.Context.TestCases.Count))
            throw new InvalidContextException("Invalid test case numbers were given as input");
    }

    /// <summary>
    /// Executes the provided test cases using the defined pipeline and converts them to their completed states.
    /// </summary>
    /// <param name="testNumbersHash">A set of integers representing the hash of the test numbers to be executed.</param>
    /// <returns>A list of completed test cases after processing through the pipeline.</returns>
    private IList<ITestCase> ExecuteTestCases(ISet<int> testNumbersHash)
    {
        var testCasePipeline = new TestCasePipeline(testNumbersHash);
        return _contextContainer.Context.TestCases
            .Select(testCase => testCasePipeline.ToCompleted(testCase))
            .ToList();
    }

    /// <summary>
    /// Creates a test suite run result based on the provided context, completed test cases, and context validation.
    /// </summary>
    private static TestSuiteRunResult GetTestSuiteRunResult(
        ITestSuiteContext context,
        IList<ITestCase> completedTestCases,
        bool shouldBeExecuted,
        Exception? exception = null)
    {
        return new TestSuiteRunResult(completedTestCases,
            context.Operation,
            context.Name,
            context.Number,
            exception: exception,
            shouldBeExecuted: shouldBeExecuted);
    }
}