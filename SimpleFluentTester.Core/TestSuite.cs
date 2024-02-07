using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester;

public static class TestSuite
{
    public static TestRunOperationBuilder Setup() => new(new DefaultTestRunReporterFactory());

    public static TestSuiteCustomBuilder Custom => new();
}