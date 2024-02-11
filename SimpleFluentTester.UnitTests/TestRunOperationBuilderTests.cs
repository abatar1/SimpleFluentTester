using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.UnitTests;

public class TestRunOperationBuilderTests
{
    [Fact]
    public void UseOperation_InvalidReturnType_ShouldThrow()
    {
        // Arrange
        var setup = TestSuite.WithExpectedReturnType<string>();
        
        // Act
        var useOperationFunc = () => setup.UseOperation(StaticMethods.Adder);

        // Assert
        Assert.Throws<InvalidCastException>(useOperationFunc);
    }
    
    [Fact]
    public void UseOperation_NoReturn_ShouldThrow()
    {
        // Arrange
        var setup = TestSuite.WithExpectedReturnType<int>();
        
        // Act
        var useOperationFunc = () => setup.UseOperation(StaticMethods.Empty);

        // Assert
        Assert.Throws<InvalidCastException>(useOperationFunc);
    }
    
    [Fact]
    public void UseOperation_ValidReturnType_ShouldBeValid()
    {
        // Arrange
        Delegate @delegate = StaticMethods.Adder;
        var expectedTestRunBuilder = new TestRunBuilder<int>(new DefaultTestRunReporterFactory(), new EntryAssemblyProvider());
        
        // Act
        var testRunBuilder = TestSuite.WithExpectedReturnType<int>().UseOperation(@delegate);

        // Assert
        Assert.Equivalent(expectedTestRunBuilder, testRunBuilder, strict: true);
    }
}