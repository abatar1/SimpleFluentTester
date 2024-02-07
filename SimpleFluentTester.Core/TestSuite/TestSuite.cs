using SimpleFluentTester.Core.Reporter;
using SimpleFluentTester.Core.TestRun;

namespace SimpleFluentTester.Core.TestSuite;

public static class TestSuite
{
    public static TestRunOperationBuilder Setup() => new(new DefaultTestRunReporterFactory());

    public static TestSuiteCustomBuilder Custom => new();
}