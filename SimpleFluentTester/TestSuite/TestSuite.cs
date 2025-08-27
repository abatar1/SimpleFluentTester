using System.Collections.Generic;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Represents a container for creating and managing test suites.
/// </summary>
public static class TestSuite
{
    private static int _testSuiteNumber;
    private static ITestSuiteController _controller = new TestSuiteController();

    /// <summary>
    /// Ignores the specified test suite numbers, preventing their execution.
    /// <b>MUST</b> be called before any test suite is created.
    /// </summary>
    /// <param name="testSuiteNumber">An array of test suite numbers to ignore.</param>
    public static void Ignore(params int[] testSuiteNumber) => _controller.Ignore(testSuiteNumber);

    /// <summary>
    /// Ignores the specified test suite names, preventing their execution.
    ///  <b>MUST</b> be called before any test suite is created.
    /// </summary>
    /// <param name="testSuiteNames">An array of test suite names to ignore.</param>
    public static void Ignore(params string[] testSuiteNames) => _controller.Ignore(testSuiteNames);

    /// <summary>
    /// Allows execution of the specified test suite numbers.
    /// <b>MUST</b> be called before any test suite is created.
    /// </summary>
    /// <param name="testSuiteNumber">An array of test suite numbers to allow.</param>
    public static void Allow(params int[] testSuiteNumber) => _controller.Allow(testSuiteNumber);

    /// <summary>
    /// Allows execution of the specified test suite names.
    /// <b>MUST</b> be called before any test suite is created.
    /// </summary>
    /// <param name="testSuiteNames">An array of test suite names to allow.</param>
    public static void Allow(params string[] testSuiteNames) => _controller.Allow(testSuiteNames);
    
    /// <summary>
    /// Sets the specified controller as the global controller for managing the test suite.
    /// </summary>
    /// <param name="controller">An instance of <see cref="ITestSuiteController"/> to be used as the global controller.</param>
    internal static void WithController(ITestSuiteController controller) => _controller = controller;

    /// <summary>
    /// Determines whether the specified test suite context is allowed for execution.
    /// </summary>
    /// <param name="context">The test suite context to evaluate.</param>
    internal static bool IsAllowed(ITestSuiteContext context) => _controller.IsAllowed(context);

    /// <summary>
    /// Provides a sequential test suite builder for creating and managing test cases in a structured sequence.
    /// </summary>
    /// <remarks>
    /// The property initializes a new sequential test suite context upon access, ensuring that each suite is
    /// uniquely identified and managed independently. It supports setting expectations, defining inputs,
    /// and creating test cases in a sequential order for execution.
    /// </remarks>
    /// <returns>
    /// An implementation of <see cref="ITestSuiteBuilder"/> that allows configuration and execution of
    /// sequential test suites.
    /// </returns>
    public static ITestSuiteBuilder Sequential
    {
        get
        {
            _testSuiteNumber += 1;
            var container = TestSuiteContextContainer.Default(_testSuiteNumber);
            return new SequentialTestSuiteBuilder(container, new List<DefinedTestClause>());
        }
    }
}