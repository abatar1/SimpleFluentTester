using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class IgnoredTests
{
    [Fact]
    public void Ignored_AddedTestCasesAndIgnoredThem_TestCasesShouldBeIgnored()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential.Ignore
            .UseAdderOperation()
            .ExpectReturn(2).WithInput(1, 1, 1)
            .ExpectReturn(3).WithInput(1, 1, 1);
            
        // Act    
        var reporter = builder.Run();
        
        // Assert
        Assert.False(reporter.TestSuiteRunResult.ShouldBeExecuted);
        reporter.AssertTestCaseExists(1).AssertIgnored();
        reporter.AssertTestCaseExists(2).AssertIgnored();
    }
}