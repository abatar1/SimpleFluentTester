using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Extensions;

public static class TestSuiteReporterExtensions
{
    public static CompletedTestCase AssertTestCaseExists(
        this ITestSuiteReporter testSuiteReporter, 
        int testNumber)
    {
        var testSuiteRunResult = testSuiteReporter.TestSuiteRunResult;
        Assert.NotNull(testSuiteRunResult);

        var testCase = testSuiteRunResult.TestCases
            .FirstOrDefault(x => x.Number == testNumber);
        Assert.NotNull(testCase);
        Assert.Equal(testNumber, testCase.Number);
        return testCase;
    }
    
    public static ITestSuiteReporter AssertInvalid(
        this ITestSuiteReporter testSuiteReporter, 
        ValidationSubject validationSubject,
        string message)
    {
        testSuiteReporter.TestSuiteRunResult.Validation.AssertInvalid(validationSubject, message);
        return testSuiteReporter;
    }
    
    public static ITestSuiteReporter AssertValid(
        this ITestSuiteReporter testSuiteReporter)
    {
        testSuiteReporter.TestSuiteRunResult.Validation.AssertValid();
        return testSuiteReporter;
    }
}