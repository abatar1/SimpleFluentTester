using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.TestCase;

/// <summary>
/// Defines an interface for building test cases in a fluent manner.
/// Provides methods to set inputs for the test case being constructed.
/// </summary>
public interface ITestCaseBuilder
{
    /// <summary>
    /// Specifies the inputs for the test case being constructed.
    /// This method allows the test case to be configured with the parameters to pass to the tested operation.
    /// </summary>
    /// <param name="inputs">The input parameters to be assigned to the test case.</param>
    /// <returns>An instance of <see cref="ITestSuiteBuilder"/> for chaining further test suite configurations.</returns>
    ITestSuiteBuilder WithInput(params object?[] inputs);

    /// <summary>
    /// Facilitates chaining additional expectation or configuration steps for constructing the current test case.
    /// This method provides a fluent interface to continue configuring the test case incrementally.
    /// </summary>
    /// <returns>An instance of <see cref="ITestSuiteBuilder"/> to enable further fluent configuration of the test suite.</returns>
    ITestSuiteBuilder And { get; }
}