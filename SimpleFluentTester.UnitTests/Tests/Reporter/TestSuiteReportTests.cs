using Microsoft.Extensions.Logging;
using Moq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.UnitTests.Helpers;

namespace SimpleFluentTester.UnitTests.Tests.Reporter;

public sealed class TestSuiteReportTests
{
    [Fact]
    public void Report_DefaultLogger_ShouldNotThrow()
    {
        // Assign
        var testSuiteResult = TestSuiteHelper.CreateTestSuiteResult(
            ValidationResults.Valid,
            TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter(testSuiteResult);
        
        // Act
        testSuiteReporter.Report();

        // Assert
    }
    
    [Fact]
    public void Report_ValidResultLogAsInformation_ShouldBeCalledOnce()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestSuiteHelper.CreateTestSuiteResult(
            ValidationResults.Valid,
            TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter(testSuiteResult);
        
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
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
    
    [Fact]
    public void Report_InvalidResultLogAsError_ShouldBeCalledOnce()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestSuiteHelper.CreateTestSuiteResult(
            ValidationResults.NonValid,
            TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter(testSuiteResult);
        
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
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
    
    [Fact]
    public void Report_CustomReporterThrowsException_ShouldBeCalledOnce()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestSuiteHelper.CreateTestSuiteResult(
            ValidationResults.Valid,
            TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder>();
        var exception = new Exception("Test");
        reportBuilder
            .Setup(x => x.TestSuiteResultToString(testSuiteResult, 
                It.IsAny<Func<CompletedTestCase, bool>>()))
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
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
    
    [Fact]
    public void Report_CustomReporterReturnNull_LogShouldNotCall()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestSuiteHelper.CreateTestSuiteResult(
            ValidationResults.Valid,
            TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder>();
        reportBuilder
            .Setup(x => 
                x.TestSuiteResultToString(testSuiteResult, It.IsAny<Func<CompletedTestCase, bool>>()))
            .Returns((TestSuiteResult _, Func<CompletedTestCase, bool> _) => null);
        
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
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }
    
    [Fact]
    public void Report_CustomReporterReturnString_LogShouldBeCalledOnce()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestSuiteHelper.CreateTestSuiteResult(
            ValidationResults.Valid,
            TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder>();
        var result = new PrintableTestSuiteResult(LogLevel.Information, 1, "Test");
        reportBuilder
            .Setup(x => x.TestSuiteResultToString(testSuiteResult, 
                It.IsAny<Func<CompletedTestCase, bool>>()))
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
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
    
    [Fact]
    public void Report_ShouldPrintPredicateSet_PredicatePassed()
    {
        // Assign
        var loggerMock = new Mock<ILogger>();
        var testSuiteResult = TestSuiteHelper.CreateTestSuiteResult(
            ValidationResults.Valid,
            TestCaseOperations.Passed);
        var testSuiteReporter = new TestSuiteReporter(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder>();
        var exception = new Exception("Test");
        var shouldPrintPredicateMock = new Mock<Func<CompletedTestCase, bool>>();
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
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}