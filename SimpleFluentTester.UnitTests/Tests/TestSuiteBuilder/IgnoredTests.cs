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
            .ExpectResult(2).WithInput(1, 1, 1)
            .ExpectResult(3).WithInput(1, 1, 1);
            
        // Act    
        var reporter = builder.Run();
        
        // Assert
        Assert.False(reporter.TestSuiteRunResult.ShouldBeExecuted);
        reporter.AssertTestCaseExists(1).AssertSkippedTestResult(2, [1, 1, 1]);
        reporter.AssertTestCaseExists(2).AssertSkippedTestResult(3, [1, 1, 1]);
    }
}