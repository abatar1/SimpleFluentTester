using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests;

public class TestRunReporterTests
{
    [Fact]
    public void DefaultReporter_NoTestCases_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite.Sequential
            .WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder)
            .Run()
            .Report();
            
        // Arrange
    }
    
    [Fact]
    public void DefaultReporter_NoFailedCases_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite.Sequential
            .WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder)
            .Expect(2).WithInput(1, 1)
            .Run()
            .Report();
            
        // Arrange
    }
    
    [Fact]
    public void DefaultReporter_WithFailedCases_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite.Sequential
            .WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder)
            .Expect(3).WithInput(1, 1)
            .Run()
            .Report();
            
        // Arrange
    }
    
    [Fact]
    public void CustomReporter_NoFailedCases_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite.Sequential
            .WithExpectedReturnType<int>()
            .WithCustomReporterFactory<CustomReporterFactory>()
            .UseOperation(StaticMethods.Adder)
            .Expect(2).WithInput(1, 1)
            .Run()
            .Report();
            
        // Arrange
    }
    
    [Fact]
    public void CustomReporter_WithAllSkippedTestCase_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite.Sequential.Ignore
            .WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder)
            .Expect(2).WithInput(1, 1)
            .Run(1)
            .Report();
            
        // Arrange
    }
    
    [Fact]
    public void TestCase_CreateMultipleInputsAndToString_ShouldNotThrow()
    {
        // Assign
        var assert = new Assert<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero);
        var testCase = new TestCase<int>(new[] { 1, 2 }.Cast<object>().ToArray(), 3, 1);
        var reporter = CreateReporterFromTestCase(assert, testCase);

        // Act
        reporter.Report();

        // Assert
    }
    
    [Fact]
    public void TestCase_CreateSingleInputAndToString_ShouldNotThrow()
    {
        // Assign
        var assert = new Assert<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero);
        var testCase = new TestCase<int>(new[] { 1 }.Cast<object>().ToArray(), 3, 1);
        var reporter = CreateReporterFromTestCase(assert, testCase);
        
        // Act
        reporter.Report();

        // Assert
    }
    
    [Fact]
    public void TestCase_ValueNotCreated_ShouldNotThrow()
    {
        // Assign
        var assert = new Assert<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero);
        var testCase = new TestCase<int>(new[] { 1 }.Cast<object>().ToArray(), 3, 1);
        var reporter = CreateReporterFromTestCase(assert, testCase);

        // Act
        reporter.Report();

        // Assert
    }

    private static ITestRunReporter CreateReporterFromTestCase<TOutput>(Assert<TOutput> assert, TestCase<TOutput> testCase, string? name = null)
    {
        var validatedTestCase = new CompletedTestCase<TOutput>(assert, ValidationStatus.Valid, testCase, new List<ValidationResult>());
        var runResult = new TestSuiteResult<TOutput>(
            new List<CompletedTestCase<TOutput>> { validatedTestCase },
            new List<ValidationResult>(), 
            StaticMethods.AdderMethodInfo, 
            name,
            0);
        return new DefaultTestRunReporter<TOutput>(runResult);
    }

    private class CustomReporterFactory : ITestRunReporterFactory
    {
        public ITestRunReporter GetReporter<TOutput>(TestSuiteResult<TOutput> testRunResult)
        {
            return new CustomReporter<TOutput>(testRunResult);
        }
    }
    
    private class CustomReporter<TOutput>(TestSuiteResult<TOutput> testRunResult) : BaseTestRunReporter<TOutput>(testRunResult)
    {
        protected override void ReportInternal()
        {
        }
    }
}