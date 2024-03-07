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
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            new ValidationResult(ValidationStatus.NonValid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 1, 1));
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
    public void TestSuiteResultToString_ValidContext_ReturnInformation()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            new ValidationResult(ValidationStatus.Valid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 1, 1));
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Information, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
    }

    [Fact]
    public void TestSuiteResultToString_TestCaseNotPassedWithException_ReturnError()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            new ValidationResult(ValidationStatus.Valid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>(["test"], 1, 1));
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
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            new ValidationResult(ValidationStatus.Valid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 2, 1));
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
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            new ValidationResult(ValidationStatus.Valid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 1, 1));
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Information, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
    }
    
    [Fact]
    public void TestSuiteResultToString_IgnoredTestSuite_ShouldBeNull()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            new ValidationResult(ValidationStatus.Valid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 1, 1),
            true);
        var reporter = new DefaultTestSuiteReportBuilder<object>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult);

        // Assert
        Assert.Null(stringResult);
    }
}