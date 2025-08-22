using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Helpers.Extensions;

public static class TestSuiteReporterExtensions
{
    public static AssertedTestCase AssertTestCaseExists(this ITestSuiteReporter testSuiteReporter, int testNumber)
    {
        var testSuiteRunResult = testSuiteReporter.TestSuiteRunResult;
        Assert.NotNull(testSuiteRunResult);

        var testCase = testSuiteRunResult.TestCases
            .FirstOrDefault(x => x.Number == testNumber);
        Assert.NotNull(testCase);
        Assert.Equal(testNumber, testCase.Number);
        return testCase;
    }
    
    public static ITestSuiteReporter AssertNonValid(this ITestSuiteReporter testSuiteReporter, ValidationSubject validationSubject, string message)
    {
        foreach (var testCase in testSuiteReporter.TestSuiteRunResult.TestCases)
            testCase.AssertNonValid(validationSubject, message);
        return testSuiteReporter;
    }
    
    public static ITestSuiteReporter AssertValid(this ITestSuiteReporter testSuiteReporter)
    {
        foreach (var testCase in testSuiteReporter.TestSuiteRunResult.TestCases)
            testCase.AssertValid();
        return testSuiteReporter;
    }
}