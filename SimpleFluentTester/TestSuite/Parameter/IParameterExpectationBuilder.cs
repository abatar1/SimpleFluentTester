using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite.Parameter;

public interface IParameterExpectationBuilder
{
    /// <summary>
    /// Specifies the expected value for the parameter.
    /// </summary>
    /// <typeparam name="T">The type of the expected value</typeparam>
    /// <param name="expected">The expected value</param>
    /// <returns>An instance of <see cref="ITestCaseBuilder"/> for further configuration.</returns>
    ITestCaseBuilder ToBe<T>(T expected);
}