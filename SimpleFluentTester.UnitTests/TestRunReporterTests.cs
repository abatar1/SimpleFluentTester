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
        var lazyResult = new LazyAssert<int>(() => new Assert<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero));
        _ = lazyResult.Value;
        var testCase = new TestCase<int>(new[] { 1, 2 }.Cast<object>().ToArray(), 3, lazyResult, 1);
        var reporter = CreateReporterFromTestCase(testCase);

        // Act
        reporter.Report();

        // Assert
    }
    
    [Fact]
    public void TestCase_CreateSingleInputAndToString_ShouldNotThrow()
    {
        // Assign
        var lazyResult = new LazyAssert<int>(() => new Assert<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero));
        _ = lazyResult.Value;
        var testCase = new TestCase<int>(new[] { 1 }.Cast<object>().ToArray(), 3, lazyResult, 1);
        var reporter = CreateReporterFromTestCase(testCase);
        
        // Act
        reporter.Report();

        // Assert
    }
    
    [Fact]
    public void TestCase_ValueNotCreated_ShouldNotThrow()
    {
        // Assign
        var lazyResult = new LazyAssert<int>(() => new Assert<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero));
        var testCase = new TestCase<int>(new[] { 1 }.Cast<object>().ToArray(), 3, lazyResult, 1);
        var reporter = CreateReporterFromTestCase(testCase);

        // Act
        reporter.Report();

        // Assert
    }

    private static ITestRunReporter CreateReporterFromTestCase<TOutput>(TestCase<TOutput> testCase)
    {
        var validatedTestCase = new ValidatedTestCase<TOutput>(ValidationStatus.Valid, testCase, new List<ValidationResult>());
        var runResult = new TestRunResult<TOutput>(
            new List<ValidatedTestCase<TOutput>> { validatedTestCase },
            new List<ValidationResult>(), 
            StaticMethods.AdderMethodInfo);
        return new DefaultTestRunReporter<TOutput>(runResult);
    }

    private class CustomReporterFactory : ITestRunReporterFactory
    {
        public ITestRunReporter GetReporter<TOutput>(TestRunResult<TOutput> testRunResult)
        {
            return new CustomReporter<TOutput>(testRunResult);
        }
    }
    
    private class CustomReporter<TOutput>(TestRunResult<TOutput> testRunResult) : BaseTestRunReporter<TOutput>(testRunResult)
    {
        protected override void ReportInternal()
        {
        }
    }
}