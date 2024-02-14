using System.Collections;
using System.Reflection;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.UnitTests;

public class TestRunReporterTests
{
    [Fact]
    public void DefaultReporter_NoTestCases_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite
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
        TestSuite
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
        TestSuite
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
        TestSuite
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
        TestSuite.Ignore
            .WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder)
            .Expect(2).WithInput(1, 1)
            .Run(1)
            .Report();
            
        // Arrange
    }

    private class CustomReporterFactory : BaseTestRunReporterFactory
    {
        public override ITestRunReporter GetReporter<TOutput>(IEnumerable testCases, MethodInfo methodInfo)
        {
            return new CustomReporter(testCases, methodInfo);
        }
    }
    
    private class CustomReporter(IEnumerable innerTestResults, MethodInfo methodInfo) : BaseTestRunReporter<int>(innerTestResults, methodInfo)
    {
        public override void Report()
        {
        }
    }
}