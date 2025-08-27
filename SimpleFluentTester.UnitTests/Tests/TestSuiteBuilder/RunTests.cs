using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class TestNumbersTests
{
    [Fact]
    public void Run_InvalidIterationNumber_ShouldThrow()
    {
        // Assign
        var builder1 = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
        var builder2 = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
        
        // Act
        Action fun1 = () => builder1
            .ExpectReturn(2).WithInput(1, 1)
            .Run(2);
        Action fun2 = () => builder2
            .ExpectReturn(2).WithInput(1, 1)
            .Run(1, 2);
        
        // Assert
        const string message = "Invalid test case numbers were given as input";
        TestHelpers.AssertWithMessage<InvalidContextException>(fun1, message);
        TestHelpers.AssertWithMessage<InvalidContextException>(fun2, message);
    }
    
    [Fact]
    public void Run_ValidIterationNumber_ShouldBeValid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
        
        // Act
        var result = builder
            .ExpectReturn(2).WithInput(1, 1)
            .Run(1);
        
        // Assert
        Assert.NotNull(result);
    }
}