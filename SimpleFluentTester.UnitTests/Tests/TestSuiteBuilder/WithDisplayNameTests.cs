using SimpleFluentTester.UnitTests.Helpers;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public class WithDisplayNameTests
{
    [Fact]
    public void WithDisplayName_NotEmptyString_TestSuiteShouldHaveCustomName()
    {
        // Assign
        var displayName = "test";
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation()
            .WithDisplayName(displayName);
            
        // Act    
        var reporter = builder.Run();
        
        // Assert
        Assert.Equal(displayName, reporter.TestSuiteResult.DisplayName);
    }
}