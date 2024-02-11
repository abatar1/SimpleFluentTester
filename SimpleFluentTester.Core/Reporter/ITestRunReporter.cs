namespace SimpleFluentTester.Reporter;

public interface ITestRunReporter
{
    /// <summary>
    /// Prints the results of test execution using the standard reporter or, if specified, using a custom reporter.
    /// </summary>
    void Report();
}