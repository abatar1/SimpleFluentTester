namespace SimpleFluentTester.TestSuite.Case;

/// <inheritdoc cref="TestCaseBuilder"/>
public interface ITestCaseBuilder
{
    /// <inheritdoc cref="TestCaseBuilder.WithInput"/>
    ITestSuiteBuilder WithInput(params object?[] inputs);
}