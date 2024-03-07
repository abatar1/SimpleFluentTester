using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests;

public class DefaultTestSuiteReportBuilderTests
{
    [Fact]
    public void TestSuiteResultToString_IgnoredResult_ReturnNull()
    {
        // Assign
        var context = TestHelpers.CreateEmptyContext<object>();
        var testSuiteResult = new TestSuiteResult<object>(
            new List<CompletedTestCase<object>>(),
            new Dictionary<ValidationSubject, IList<ValidationResult>>(),
            context.Operation,
            context.Name,
            context.Number,
            true);
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.Null(stringResult);
    }

    [Fact]
    public void TestSuiteResultToString_NoTestCases_ReturnError()
    {
        // Assign
        var context = TestHelpers.CreateEmptyContext<object>();
        var testSuiteResult = new TestSuiteResult<object>(
            new List<CompletedTestCase<object>>(),
            new Dictionary<ValidationSubject, IList<ValidationResult>>(),
            context.Operation,
            context.Name,
            context.Number);
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
    }

    [Fact]
    public void TestSuiteResultToString_NonValidContext_ReturnError()
    {
        // Assign
        var validationResults = GetValidationResults(ValidationStatus.NonValid);
        var context = TestHelpers.CreateEmptyContext<object>(operation: (int x) => x);
        var testCase = new TestCase<object>([1], 1, 1);
        var testCaseExecutor = new TestCaseExecutor<object>(context);
        var completedTestCase = testCaseExecutor.TryCompeteTestCase(testCase, new SortedSet<int>([1]));
        var testSuiteResult = new TestSuiteResult<object>(
            new List<CompletedTestCase<object>> { completedTestCase },
            validationResults,
            context.Operation,
            context.Name,
            context.Number);
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
    }

    [Fact]
    public void TestSuiteResultToString_TestCaseNotPassedWithException_ReturnError()
    {
        // Assign
        var validationResults = GetValidationResults(ValidationStatus.Valid);
        var context = TestHelpers.CreateEmptyContext<object>(operation: (int x) => x);
        var testCase = new TestCase<object>(["test"], 2, 1);
        var testCaseExecutor = new TestCaseExecutor<object>(context);
        var completedTestCase = testCaseExecutor.TryCompeteTestCase(testCase, new SortedSet<int>([1]));
        var testSuiteResult = new TestSuiteResult<object>(
            new List<CompletedTestCase<object>> { completedTestCase },
            validationResults,
            context.Operation,
            context.Name,
            context.Number);
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
    }

    [Fact]
    public void TestSuiteResultToString_TestCaseNotPassed_ReturnError()
    {
        // Assign
        var validationResults = GetValidationResults(ValidationStatus.Valid);
        var context = TestHelpers.CreateEmptyContext<object>(operation: (int x) => x);
        var testCase = new TestCase<object>([1], 2, 1);
        var testCaseExecutor = new TestCaseExecutor<object>(context);
        var completedTestCase = testCaseExecutor.TryCompeteTestCase(testCase, new SortedSet<int>([1]));
        var testSuiteResult = new TestSuiteResult<object>(
            new List<CompletedTestCase<object>> { completedTestCase },
            validationResults,
            context.Operation,
            context.Name,
            context.Number);
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
    }

    [Fact]
    public void TestSuiteResultToString_TestCasePassed_ReturnInformation()
    {
        // Assign
        var validationResults = GetValidationResults(ValidationStatus.Valid);
        var context = TestHelpers.CreateEmptyContext<object>(operation: (int x) => x);
        var testCase = new TestCase<object>([1], 1, 1);
        var testCaseExecutor = new TestCaseExecutor<object>(context);
        var completedTestCase = testCaseExecutor.TryCompeteTestCase(testCase, new SortedSet<int>([1]));
        var testSuiteResult = new TestSuiteResult<object>(
            new List<CompletedTestCase<object>> { completedTestCase },
            validationResults,
            context.Operation,
            context.Name,
            context.Number);
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Information, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
    }

    private static Dictionary<ValidationSubject, IList<ValidationResult>> GetValidationResults(ValidationStatus status)
    {
        return new Dictionary<ValidationSubject, IList<ValidationResult>>
        {
            {
                ValidationSubject.Operation,
                new List<ValidationResult> { new(status, ValidationSubject.Operation) }
            }
        };
    }
}