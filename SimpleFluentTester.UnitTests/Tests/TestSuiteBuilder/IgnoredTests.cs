using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public class IgnoredTests
{
    [Fact]
    public void Ignored_AddedTestCasesAndIgnoredThem_TestCasesShouldBeIgnored()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential.Ignore
            .UseAdderOperation()
            .Expect(2).WithInput(1, 1, 1)
            .Expect(3).WithInput(1, 1, 1);
            
        // Act    
        var reporter = builder.Run();
        
        // Assert
        Assert.False(reporter.TestSuiteRunResult.ShouldBeExecuted);
        reporter.AssertTestCaseExists(1).AssertSkippedTestResult(2, [1, 1, 1]);
        reporter.AssertTestCaseExists(2).AssertSkippedTestResult(3, [1, 1, 1]);
    }
}