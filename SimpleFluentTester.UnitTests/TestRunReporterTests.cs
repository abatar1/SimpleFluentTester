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
        TestSuite.Setup()
            .UseOperation<int>(StaticMethods.Adder)
            .Run()
            .Report();
            
        // Arrange
    }
    
    [Fact]
    public void DefaultReporter_NoFailedCases_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite.Setup()
            .UseOperation<int>(StaticMethods.Adder)
            .AddTestCase(2, 1, 1)
            .Run()
            .Report();
            
        // Arrange
    }
    
    [Fact]
    public void DefaultReporter_WithFailedCases_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite.Setup()
            .UseOperation<int>(StaticMethods.Adder)
            .AddTestCase(3, 1, 1)
            .Run()
            .Report();
            
        // Arrange
    }
    
    [Fact]
    public void CustomReporter_NoReportActions_ShouldNotThrow()
    {
        // Assign
        
        // Act
        TestSuite.Custom
            .WithCustomReporterFactory<CustomReporterFactory>()
            .Setup()
            .UseOperation<int>(StaticMethods.Adder)
            .AddTestCase(3, 1, 1)
            .Run()
            .Report();
            
        // Arrange
    }

    private class CustomReporterFactory : BaseTestRunReporterFactory
    {
        public override ITestRunReporter GetReporter<TOutput>(IList testCases, MethodInfo methodInfo)
        {
            return new CustomReporter(testCases, methodInfo);
        }
    }
    
    private class CustomReporter(IList innerTestResults, MethodInfo methodInfo) : BaseTestRunReporter<int>(innerTestResults, methodInfo)
    {
        public override void Report()
        {
        }
    }
}