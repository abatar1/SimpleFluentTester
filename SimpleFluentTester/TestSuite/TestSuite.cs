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

    /// <summary>
    /// Ignores the specified test suite numbers, preventing their execution.
    /// <b>MUST</b> be called before any test suite is created.
    /// </summary>
    /// <param name="testSuiteNumber">An array of test suite numbers to ignore.</param>
    public static void Ignore(params int[] testSuiteNumber) => TestSuiteGlobalState.Ignore(testSuiteNumber);

    /// <summary>
    /// Ignores the specified test suite names, preventing their execution.
    ///  <b>MUST</b> be called before any test suite is created.
    /// </summary>
    /// <param name="testSuiteNames">An array of test suite names to ignore.</param>
    public static void Ignore(params string[] testSuiteNames) => TestSuiteGlobalState.Ignore(testSuiteNames);

    /// <summary>
    /// Allows execution of the specified test suite numbers.
    /// <b>MUST</b> be called before any test suite is created.
    /// </summary>
    /// <param name="testSuiteNumber">An array of test suite numbers to allow.</param>
    public static void Allow(params int[] testSuiteNumber) => TestSuiteGlobalState.Allow(testSuiteNumber);

    /// <summary>
    /// Allows execution of the specified test suite names.
    /// <b>MUST</b> be called before any test suite is created.
    /// </summary>
    /// <param name="testSuiteNames">An array of test suite names to allow.</param>
    public static void Allow(params string[] testSuiteNames) => TestSuiteGlobalState.Allow(testSuiteNames);

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