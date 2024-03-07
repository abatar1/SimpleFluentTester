
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
            new ValidationResult(ValidationStatus.Valid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 1, 1));
        var testSuiteReporter = new TestSuiteReporter<object>(testSuiteResult);
        
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
            new ValidationResult(ValidationStatus.Valid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 1, 1));
        var testSuiteReporter = new TestSuiteReporter<object>(testSuiteResult);
        
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
            new ValidationResult(ValidationStatus.NonValid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 1, 1));
        var testSuiteReporter = new TestSuiteReporter<object>(testSuiteResult);
        
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
            new ValidationResult(ValidationStatus.Valid, ValidationSubject.Operation),
            (int x) => x,
            new TestCase<object>([1], 1, 1));
        var testSuiteReporter = new TestSuiteReporter<object>(testSuiteResult);
        var reportBuilder = new Mock<ITestSuiteReportBuilder<object>>();
        var exception = new Exception("Test");
        reportBuilder
            .Setup(x => x.TestSuiteResultToString(testSuiteResult))
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
}