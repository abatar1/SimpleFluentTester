using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.UnitTests;

public class TestRunOperationBuilderTests
{
    [Fact]
    public void UseOperation_InvalidReturnType_ShouldThrow()
    {
        // Arrange
        var setup = TestSuite.Setup();
        
        // Act
        var useOperationFunc = () => setup.UseOperation<string>(StaticMethods.Adder);

        // Assert
        Assert.Throws<InvalidCastException>(useOperationFunc);
    }
    
    [Fact]
    public void UseOperation_NullDelegate_ShouldThrow()
    {
        // Arrange
        var setup = TestSuite.Setup();
        
        // Act
        var useOperationFunc = () => setup.UseOperation<int>(null);

        // Assert
        Assert.Throws<ArgumentNullException>(useOperationFunc);
    }
    
    [Fact]
    public void UseOperation_NoReturn_ShouldThrow()
    {
        // Arrange
        var setup = TestSuite.Setup();
        
        // Act
        var useOperationFunc = () => setup.UseOperation<int>(StaticMethods.Empty);

        // Assert
        Assert.Throws<InvalidCastException>(useOperationFunc);
    }
    
    [Fact]
    public void UseOperation_ValidReturnType_ShouldBeValid()
    {
        // Arrange
        Delegate @delegate = StaticMethods.Adder;
        var expectedTestRunBuilder = new TestRunBuilder<int>(@delegate, new DefaultTestRunReporterFactory());
        
        // Act
        var testRunBuilder = TestSuite.Setup().UseOperation<int>(@delegate);

        // Assert
        Assert.Equivalent(expectedTestRunBuilder, testRunBuilder, strict: true);
    }
}