using System;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.Parameter;

namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Represents a builder interface for configuring and executing test suites.
/// Provides methods to define test cases, set expectations, configure operations,
/// specify test case display names, and execute the tests with specified configurations.
/// </summary>
public interface ITestSuiteBuilder
{
    /// <inheritdoc cref="TestSuiteBuilder.ExpectResult"/>
    /// <summary>
    /// Specifies the expected value resulting from the execution of this test case.
    /// </summary>
    /// <param name="expected">The value expected as the result of executing the test case.</param>
    /// <returns>An instance of <see cref="ITestCaseBuilder"/> for further configuration of the test case.</returns>
    ITestCaseBuilder ExpectResult(object? expected);

    /// <summary>
    /// Specifies the expected exception type and optional message that should be thrown during the execution of the test case.
    /// </summary>
    /// <typeparam name="TException">The type of exception expected to be thrown.</typeparam>
    /// <param name="message">An optional message to validate against the exception being thrown.</param>
    /// <returns>An instance of <see cref="ITestCaseBuilder"/> for further configuration of the test case.</returns>
    ITestCaseBuilder ExpectException<TException>(string? message = null)
        where TException : Exception;
    
    /// <summary>
    /// Initiates expectation configuration for a parameter identified by name or position.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to verify</param>
    /// <returns>An instance of <see cref="IParameterExpectationBuilder"/> for specifying the expected value.</returns>
    IParameterExpectationBuilder ExpectParameter(string parameterName);

    /// <summary>
    /// Initiates expectation configuration for a parameter at the specified position.
    /// </summary>
    /// <param name="parameterPosition">Zero-based position of the parameter</param>
    /// <returns>An instance of <see cref="IParameterExpectationBuilder"/> for specifying the expected value.</returns>
    IParameterExpectationBuilder ExpectParameter(int parameterPosition);

    /// <summary>
    /// Specifies the operation to be executed as part of the test suite.
    /// </summary>
    /// <param name="operation">The delegate representing the operation to be executed.</param>
    /// <returns>An instance of <see cref="ITestSuiteBuilder"/> for further configuration of the test suite.</returns>
    ITestSuiteBuilder UseOperation(Delegate operation);

    /// <summary>
    /// Specifies the display name of the test suite to be used in the output.
    /// </summary>
    /// <param name="displayName">The custom display name to be assigned to the test suite.</param>
    /// <returns>An instance of <see cref="ITestSuiteBuilder"/> for further configuration of the test suite.</returns>
    ITestSuiteBuilder WithDisplayName(string displayName);

    /// <summary>
    /// Configures a custom comparer to be used for comparing expected and actual values during test evaluation.
    /// </summary>
    /// <param name="comparer">A function that defines how two values of type <typeparamref name="TExpected"/> should be compared for equality.</param>
    /// <typeparam name="TExpected">The type of values being compared by the custom comparer.</typeparam>
    /// <returns>An instance of <see cref="ITestSuiteBuilder"/> for further configuration of the test suite.</returns>
    ITestSuiteBuilder WithComparer<TExpected>(ComparerDelegate<TExpected> comparer);
    
    /// <summary>
    /// Gets a builder instance configured to ignore the test suite execution.
    /// By calling this property, the builder will mark the entire test suite as ignored.
    /// This allows test cases to be defined and configured but prevents them from being executed during the test run.
    /// </summary>
    /// <returns>An instance of <see cref="ITestSuiteBuilder"/> for further configuration of the test suite.</returns>
    ITestSuiteBuilder Ignore { get; }

    /// <summary>
    /// Executes the defined test suite using the specified test case numbers or all test cases if no numbers are provided.
    /// </summary>
    /// <param name="testNumbers">The specific test case numbers to run. If no numbers are passed, all test cases in the suite will be executed.</param>
    /// <returns>An instance of <see cref="ITestSuiteReporter"/> that contains the results of the test suite execution.</returns>
    ITestSuiteReporter Run(params int[] testNumbers);
}