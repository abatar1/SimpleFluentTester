using Microsoft.Extensions.Logging;
using Moq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.UnitTests.Helpers;

namespace SimpleFluentTester.UnitTests.Tests.Reporter;

public sealed class DefaultTestSuiteReportBuilderTests
{
    [Fact]
    public void TestSuiteResultToString_IgnoredResult_ReturnNull()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(shouldBeExecuted: false);
        var reporter = new DefaultTestSuiteReportBuilder();
        var shouldPrintPredicateMock = new Mock<Func<AssertedTestClause, AssertedTestCase, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.Null(stringResult);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<AssertedTestClause>(), It.IsAny<AssertedTestCase>()), Times.Never);
    }

    [Fact]
    public void TestSuiteResultToString_NoTestCases_ReturnError()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult();
        var reporter = new DefaultTestSuiteReportBuilder();
        var shouldPrintPredicateMock = new Mock<Func<AssertedTestClause, AssertedTestCase, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal("No test cases were added", stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<AssertedTestClause>(), It.IsAny<AssertedTestCase>()), Times.Never);
    }

    [Fact]
    public void TestSuiteResultToString_NonValidContext_ReturnError()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Invalid);
        var reporter = new DefaultTestSuiteReportBuilder();
        var shouldPrintPredicateMock = new Mock<Func<AssertedTestClause, AssertedTestCase, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<AssertedTestClause>(), It.IsAny<AssertedTestCase>()), Times.Once);
    }
    
    [Fact]
    public void TestSuiteResultToString_ValidContext_ReturnInformation()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Passed);
        var reporter = new DefaultTestSuiteReportBuilder();
        var shouldPrintPredicateMock = new Mock<Func<AssertedTestClause, AssertedTestCase, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Information, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<AssertedTestClause>(), It.IsAny<AssertedTestCase>()), Times.Once);
    }

    [Fact]
    public void TestSuiteResultToString_TestCaseNotPassedWithException_ReturnError()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Invalid);
        var reporter = new DefaultTestSuiteReportBuilder();
        var shouldPrintPredicateMock = new Mock<Func<AssertedTestClause, AssertedTestCase, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<AssertedTestClause>(), It.IsAny<AssertedTestCase>()), Times.Once);
    }

    [Fact]
    public void TestSuiteResultToString_TestCaseNotPassed_ReturnError()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.NotPassed);
        var reporter = new DefaultTestSuiteReportBuilder();
        var shouldPrintPredicateMock = new Mock<Func<AssertedTestClause, AssertedTestCase, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<AssertedTestClause>(), It.IsAny<AssertedTestCase>()), Times.Once);
    }

    [Fact]
    public void TestSuiteResultToString_TestCasePassed_ReturnInformation()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Passed);
        var reporter = new DefaultTestSuiteReportBuilder();
        var shouldPrintPredicateMock = new Mock<Func<AssertedTestClause, AssertedTestCase, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Information, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<AssertedTestClause>(), It.IsAny<AssertedTestCase>()), Times.Once);
    }
    
    [Fact]
    public void TestSuiteResultToString_TestCaseIgnored_ShouldBeError()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Passed, testCaseToRun: 2);
        var reporter = new DefaultTestSuiteReportBuilder();
        var shouldPrintPredicateMock = new Mock<Func<AssertedTestClause, AssertedTestCase, bool>>();

        // Act
        var stringResult = reporter.TestSuiteResultToString(testSuiteResult, shouldPrintPredicateMock.Object);

        // Assert
        Assert.NotNull(stringResult);
        Assert.Equal(LogLevel.Error, stringResult.LogLevel);
        Assert.NotNull(stringResult.Message);
        Assert.Equal(testSuiteResult.Number, stringResult.EventId);
        shouldPrintPredicateMock
            .Verify(x => x.Invoke(It.IsAny<AssertedTestClause>(), It.IsAny<AssertedTestCase>()), Times.Once);
    }
}