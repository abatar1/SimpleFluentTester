using SimpleFluentTester.Reporter;
using SimpleFluentTester.Reporter.Console;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Tests.Reporter;

public sealed class ConsoleTestSuiteReporterConfigurationTests
{
    [Fact]
    public void Build_Default_AllShouldBeNotNull()
    {
        // Assign
        var builder = new ConsoleTestSuiteReporterConfigurationBuilder();
        var container = TestSuiteContextContainer.Default();
        var completedTestCase1 = TestCaseExamples.NotPassed.CompleteTestCase(container);
        var completedTestCase2 = TestCaseExamples.NonValidOperation.CompleteTestCase(container);
        var completedTestCase3 = TestCaseExamples.NotPassedWithOperationException.CompleteTestCase(container);
        var completedTestCase4 = TestCaseExamples.NotPassedWithComparerException.CompleteTestCase(container);
        var completedTestCase5 = TestCaseExamples.Passed.CompleteTestCase(container);

        // Act
        var configuration = (ConsoleTestSuiteReporterConfiguration) builder.Build();

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
        var builder = new ConsoleTestSuiteReporterConfigurationBuilder();

        // Act
        builder.WithReportBuilder<ConsoleTestSuiteReportBuilder>();
        var configuration = (ConsoleTestSuiteReporterConfiguration) builder.Build();

        // Assert
        Assert.NotNull(configuration);
        Assert.NotNull(configuration.ReportBuilder);
        Assert.IsType<ConsoleTestSuiteReportBuilder>(configuration.ReportBuilder);
        Assert.NotNull(configuration.LoggingBuilder);
        Assert.NotNull(configuration.PrintablePredicate);
    }
    
    [Fact]
    public void Build_WithPrintablePredicate_AllShouldBeNotNull()
    {
        // Assign
        var builder = new ConsoleTestSuiteReporterConfigurationBuilder();
        var container = TestSuiteContextContainer.Default();
        var completedTestCase = TestCaseExamples.Passed.CompleteTestCase(container);

        // Act
        builder.WithPrintablePredicate((_, testCase) => testCase == completedTestCase);
        var configuration = (ConsoleTestSuiteReporterConfiguration) builder.Build();

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