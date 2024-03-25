using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.UnitTests.Extensions;

public static class TestSuiteReporterExtensions
{
    public static CompletedTestCase AssertTestCaseExists(
        this ITestSuiteReporter testSuiteReporter, 
        int testNumber)
    {
        var testSuiteResult = testSuiteReporter.TestSuiteResult;
        Assert.NotNull(testSuiteResult);

        var testCase = testSuiteResult.TestCases
            .FirstOrDefault(x => x.Number == testNumber);
        Assert.NotNull(testCase);
        Assert.Equal(testNumber, testCase.Number);
        return testCase;
    }
}