using Microsoft.Extensions.Logging;
using Moq;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Tests.Reporter;

public sealed class TestSuiteReporterConfigurationBuilderTests
{
    [Fact]
    public void Build_Default_AllShouldBeNotNull()
    {
        // Assign
        var builder = new TestSuiteReporterConfigurationBuilder();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var completedTestCase1 = TestCaseExamples.NotPassed.CompleteTestCase(container);
        var completedTestCase2 = TestCaseExamples.Invalid.CompleteTestCase(container);
        var completedTestCase3 = TestCaseExamples.NotPassedWithOperationException.CompleteTestCase(container);
        var completedTestCase4 = TestCaseExamples.NotPassedWithComparerException.CompleteTestCase(container);
        var completedTestCase5 = TestCaseExamples.Passed.CompleteTestCase(container);

        // Act
        var configuration = builder.Build();

        // Assert
        Assert.NotNull(configuration);
        Assert.NotNull(configuration.ReportBuilder);
        Assert.NotNull(configuration.LoggingBuilder);
        Assert.NotNull(configuration.PrintablePredicate);
        
        AssertPredicate(configuration, completedTestCase1, true);
        AssertPredicate(configuration, completedTestCase2, true);
        AssertPredicate(configuration, completedTestCase3, true);
        AssertPredicate(configuration, completedTestCase4, true);
        AssertPredicate(configuration, completedTestCase5, false);
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
        builder.WithLoggingBuilder(loggingBuilder => { _ = loggingBuilder.Services; });

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
        var completedTestCase = TestCaseExamples.Passed.CompleteTestCase(container);

        // Act
        builder.WithPrintablePredicate((_, testCase) => testCase == completedTestCase);
        var configuration = builder.Build();

        // Assert
        Assert.NotNull(configuration);
        Assert.NotNull(configuration.ReportBuilder);
        Assert.NotNull(configuration.LoggingBuilder);
        Assert.NotNull(configuration.PrintablePredicate);
        
        AssertPredicate(configuration, completedTestCase, true);
        var anotherCompletedTestCase = TestCaseExamples.Passed.CompleteTestCase(container);
        AssertPredicate(configuration, anotherCompletedTestCase, false);
    }
    
    private static void AssertPredicate(ITestSuiteReporterConfiguration configuration, AssertedTestCase testCase, bool result)
    {
        var clause = testCase.Clauses.First();
        Assert.Equal(configuration.PrintablePredicate?.Invoke((AssertedTestClause)clause, testCase), result);
    }
}