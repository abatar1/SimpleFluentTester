using SimpleFluentTester.TestCase.Pipeline;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests;

public sealed class TestCasePipelineTests
{
    [Fact]
    public void TestCasePipeline_NonValidOperation_ShouldBeNonValid()
    {
        // Assign
        var testNumbers = new HashSet<int> { 1 };
        var pipeline = new TestCasePipeline(testNumbers);

        // Act
        var completedTestCase = pipeline.ToCompleted(TestCaseExamples.NonValidOperation);

        // Assert
        completedTestCase.AssertNonValid(ValidationSubject.Operation, TestCaseExamples.NonValidOperationMessage);
    }
    
    [Fact]
    public void TestCasePipeline_DifferTestNumbers_ShouldBeIgnored()
    {
        // Assign
        var testNumbers = new HashSet<int> { 2 };
        var pipeline = new TestCasePipeline(testNumbers);

        // Act
        var completedTestCase = pipeline.ToCompleted(TestCaseExamples.NonValidOperation);

        // Assert
        completedTestCase.AssertIgnored();
    }
    
    [Fact]
    public void TestCasePipeline_EmptyTestNumbers_ShouldBeExecuted()
    {
        // Assign
        var testNumbers = new HashSet<int>();
        var pipeline = new TestCasePipeline(testNumbers);

        // Act
        var completedTestCase = pipeline.ToCompleted(TestCaseExamples.Passed);

        // Assert
        completedTestCase.AssertPassed(3, [1, 2]);
    }
    
    [Fact]
    public void TestCasePipeline_ValidOperation_ShouldBeExecuted()
    {
        // Assign
        var testNumbers = new HashSet<int> { 1 };
        var pipeline = new TestCasePipeline(testNumbers);

        // Act
        var completedTestCase = pipeline.ToCompleted(TestCaseExamples.Passed);

        // Assert
        completedTestCase.AssertPassed(3, [1, 2]);
    }
}