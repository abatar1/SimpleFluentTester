
using Microsoft.Extensions.Logging;
using Moq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests;

public class TestSuiteReportTests
{
    [Fact]
    public void Report_DefaultLogger_ShouldNotThrow()
    {
        // Assign
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter<int>(testSuiteResult);
        
        // Act
        testSuiteReporter.Report();

        // Assert
    }
    
    [Fact]
    public void Report_ValidResultLogAsInformation_ShouldBeCalledOnce()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter<int>(testSuiteResult);
        
        // Act
        testSuiteReporter.Report((configuration, _) =>
        {
            configuration.Logger = loggerMock.Object;
        });

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Information), 
            It.IsAny<EventId>(), 
            It.IsAny<It.IsAnyType>(), 
            It.IsAny<Exception>(), 
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Once);
    }
    
    [Fact]
    public void Report_InvalidResultLogAsError_ShouldBeCalledOnce()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.NonValid,
            TestHelpers.TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter<int>(testSuiteResult);
        
        // Act
        testSuiteReporter.Report((configuration, _) =>
        {
            configuration.Logger = loggerMock.Object;
        });

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Error), 
            It.IsAny<EventId>(), 
            It.IsAny<It.IsAnyType>(), 
            It.IsAny<Exception>(), 
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Once);
    }
    
    [Fact]
    public void Report_CustomReporterThrowsException_ShouldBeCalledOnce()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter<int>(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder<int>>();
        var exception = new Exception("Test");
        reportBuilder
            .Setup(x => x.TestSuiteResultToString(testSuiteResult, 
                It.IsAny<Func<CompletedTestCase<int>, bool>>()))
            .Throws(exception);
        
        // Act
        testSuiteReporter.Report((configuration, _) =>
        {
            configuration.Logger = loggerMock.Object;
            configuration.ReportBuilder = reportBuilder.Object;
        });

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Error), 
            It.IsAny<EventId>(), 
            It.IsAny<It.IsAnyType>(), 
            It.IsAny<Exception>(), 
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Once);
    }
    
    [Fact]
    public void Report_CustomReporterReturnNull_LogShouldNotCall()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter<int>(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder<int>>();
        reportBuilder
            .Setup(x => x.TestSuiteResultToString(testSuiteResult, 
                It.IsAny<Func<CompletedTestCase<int>, bool>>()))
            .Returns<string>(null);
        
        // Act
        testSuiteReporter.Report((configuration, _) =>
        {
            configuration.Logger = loggerMock.Object;
            configuration.ReportBuilder = reportBuilder.Object;
        });

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.IsAny<LogLevel>(), 
            It.IsAny<EventId>(), 
            It.IsAny<It.IsAnyType>(), 
            It.IsAny<Exception>(), 
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Never);
    }
    
    [Fact]
    public void Report_CustomReporterReturnString_LogShouldBeCalledOnce()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter<int>(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder<int>>();
        var result = new PrintableTestSuiteResult(LogLevel.Information, 1, "Test");
        reportBuilder
            .Setup(x => x.TestSuiteResultToString(testSuiteResult, 
                It.IsAny<Func<CompletedTestCase<int>, bool>>()))
            .Returns(result);
        
        // Act
        testSuiteReporter.Report((configuration, _) =>
        {
            configuration.Logger = loggerMock.Object;
            configuration.ReportBuilder = reportBuilder.Object;
        });

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == result.LogLevel), 
            It.Is<EventId>(x => x == result.EventId), 
            It.Is<It.IsAnyType>((@object, _) => @object.ToString() == result.Message), 
            It.IsAny<Exception>(), 
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Once);
    }
    
    [Fact]
    public void Report_ShouldPrintPredicateSet_PredicatePassed()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestHelpers.GetTestSuiteResult(
            TestHelpers.ValidationResults.Valid,
            TestHelpers.TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter<int>(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder<int>>();
        var exception = new Exception("Test");
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase<int>, bool>>();
        reportBuilder
            .Setup(x => x.TestSuiteResultToString(testSuiteResult, 
                shouldPrintPredicateMock.Object))
            .Throws(exception);
        
        // Act
        testSuiteReporter.Report((configuration, _) =>
        {
            configuration.Logger = loggerMock.Object;
            configuration.ReportBuilder = reportBuilder.Object;
            configuration.ShouldPrintPredicate = shouldPrintPredicateMock.Object;
        });

        // Assert
        loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(x => x == LogLevel.Error), 
            It.IsAny<EventId>(), 
            It.IsAny<It.IsAnyType>(), 
            It.IsAny<Exception>(), 
            It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Once);
    }
}