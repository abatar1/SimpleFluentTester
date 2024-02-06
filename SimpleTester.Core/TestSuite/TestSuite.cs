using SimpleTester.Core.Reporter;
using SimpleTester.Core.TestRun;

namespace SimpleTester.Core.TestSuite;

public static class TestSuite
{
    public static TestRunOperationBuilder Setup() => new(new DefaultTestRunReporterFactory());

    public static TestSuiteCustomBuilder Custom => new();
}