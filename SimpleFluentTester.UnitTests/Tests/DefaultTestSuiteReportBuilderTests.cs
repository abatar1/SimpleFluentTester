using Microsoft.Extensions.Logging;
using Moq;
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
            new List<ValidationResult>(),
            context.Operation,
            context.Name,
            context.Number,
            ValidationStatus.Ignored);
        var reporter = new DefaultTestSuiteReportBuilder<object>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<object>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.Null(stringResult);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<object>>()), Times.Never);
    }

    [Fact]
    public void TestSuiteResultToString_NoTestCases_ReturnError()
    {
        // Assign
        var context = TestHelpers.CreateEmptyContext<object>();
        var testSuiteResult = new TestSuiteResult<object>(
            new List<CompletedTestCase<object>>(),
            new List<ValidationResult>(),
            context.Operation,
            context.Name,
            context.Number,
            ValidationStatus.Valid);
        var reporter = new DefaultTestSuiteReportBuilder<object>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<object>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal("No test cases were added", stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<object>>()), Times.Never);
    }

    [Fact]
    public void TestSuiteResultToString_NonValidContext_ReturnError()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.NonValid,
            TestHelpers.TestCaseOperations.Passed);
        var reporter = new DefaultTestSuiteReportBuilder<int>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<int>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<int>>()), Times.Once);
    }
    
    [Fact]
    public void TestSuiteResultToString_ValidContext_ReturnInformation()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed);
        var reporter = new DefaultTestSuiteReportBuilder<int>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<int>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Information, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<int>>()), Times.Once);
    }

    [Fact]
    public void TestSuiteResultToString_TestCaseNotPassedWithException_ReturnError()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Invalid);
        var reporter = new DefaultTestSuiteReportBuilder<int>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<int>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<int>>()), Times.Once);
    }

    [Fact]
    public void TestSuiteResultToString_TestCaseNotPassed_ReturnError()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.NotPassed);
        var reporter = new DefaultTestSuiteReportBuilder<int>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<int>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<int>>()), Times.Once);
    }

    [Fact]
    public void TestSuiteResultToString_TestCasePassed_ReturnInformation()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed);
        var reporter = new DefaultTestSuiteReportBuilder<int>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<int>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Information, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<int>>()), Times.Once);
    }
    
    [Fact]
    public void TestSuiteResultToString_IgnoredTestSuite_ShouldBeNull()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed,
            ignored: true);
        var reporter = new DefaultTestSuiteReportBuilder<int>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<int>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.Null(stringResult);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<int>>()), Times.Never);
    }
    
    [Fact]
    public void TestSuiteResultToString_TestCaseIgnored_ShouldBeError()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed,
            testCaseToRun: 2);
        var reporter = new DefaultTestSuiteReportBuilder<int>();
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<int>, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<CompletedTestCase<int>>()), Times.Once);
    }
}