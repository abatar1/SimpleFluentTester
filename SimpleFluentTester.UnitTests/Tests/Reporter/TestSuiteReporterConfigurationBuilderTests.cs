using Microsoft.Extensions.Logging;
using Moq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;

namespace SimpleFluentTester.UnitTests.Tests.Reporter;

public sealed class TestSuiteReporterConfigurationBuilderTests
{
    [Fact]
    public void Build_Default_AllShouldBeNotNull()
    {
        // Assign
        var builder = new TestSuiteReporterConfigurationBuilder();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var completedTestCase1 = TestCaseOperations.NotPassed.CompleteTestCase(container);
        var completedTestCase2 = TestCaseOperations.Invalid.CompleteTestCase(container);
        var completedTestCase3 = TestCaseOperations.NotPassedWithOperationException.CompleteTestCase(container);
        var completedTestCase4 = TestCaseOperations.NotPassedWithComparerException.CompleteTestCase(container);
        var completedTestCase5 = TestCaseOperations.Passed.CompleteTestCase(container);

        // Act
        var configuration = builder.Build();

        // Assert
        Assert.NotNull(configuration);
        Assert.NotNull(configuration.ReportBuilder);
        Assert.NotNull(configuration.LoggingBuilder);
        Assert.NotNull(configuration.PrintablePredicate);
        
        Assert.True(configuration.PrintablePredicate.Invoke(completedTestCase1));
        Assert.True(configuration.PrintablePredicate.Invoke(completedTestCase2));
        Assert.True(configuration.PrintablePredicate.Invoke(completedTestCase3));
        Assert.True(configuration.PrintablePredicate.Invoke(completedTestCase4));
        Assert.False(configuration.PrintablePredicate.Invoke(completedTestCase5));
    }
    
    [Fact]
    public void Build_WithReportBuilder_AllShouldBeNotNull()
    {
        // Assign
        var builder = new TestSuiteReporterConfigurationBuilder();

        // Act
        var reportBuilder = new DefaultTestSuiteReportBuilder();
        builder.WithReportBuilder(() => reportBuilder);
        var configuration = builder.Build();

        // Assert
        Assert.NotNull(configuration);
        Assert.NotNull(configuration.ReportBuilder);
        Assert.Equal(reportBuilder, configuration.ReportBuilder);
        Assert.NotNull(configuration.LoggingBuilder);
        Assert.NotNull(configuration.PrintablePredicate);
    }

    [Fact]
    public void Build_WithLoggingBuilder_AllShouldBeNotNull()
    {
        // Assign
        var builder = new TestSuiteReporterConfigurationBuilder();

        // Act
        var loggingBuilderMock = new Mock<ILoggingBuilder>();
        builder.WithLoggingBuilder(loggingBuilder =>
        {
            var services = loggingBuilder.Services;
        });

        var configuration = builder.Build();

        // Assert
        Assert.NotNull(configuration);
        Assert.NotNull(configuration.ReportBuilder);
        Assert.NotNull(configuration.LoggingBuilder);
        configuration.LoggingBuilder.Invoke(loggingBuilderMock.Object);
        loggingBuilderMock.VerifyGet(x => x.Services, Times.Once);
        Assert.NotNull(configuration.PrintablePredicate);
    }
    
    [Fact]
    public void Build_WithPrintablePredicate_AllShouldBeNotNull()
    {
        // Assign
        var builder = new TestSuiteReporterConfigurationBuilder();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var completedTestCase = TestCaseOperations.Passed.CompleteTestCase(container);

        // Act
        builder.WithPrintablePredicate(testCase => testCase == completedTestCase);
        var configuration = builder.Build();

        // Assert
        Assert.NotNull(configuration);
        Assert.NotNull(configuration.ReportBuilder);
        Assert.NotNull(configuration.LoggingBuilder);
        Assert.NotNull(configuration.PrintablePredicate);
        
        Assert.True(configuration.PrintablePredicate.Invoke(completedTestCase));
        var anotherCompletedTestCase = TestCaseOperations.Passed.CompleteTestCase(container);
        Assert.False(configuration.PrintablePredicate.Invoke(anotherCompletedTestCase));
    }
}