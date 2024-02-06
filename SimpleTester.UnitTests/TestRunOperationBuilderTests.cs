using SimpleTester.Core.Reporter;
using SimpleTester.Core.TestRun;
using SimpleTester.Core.TestSuite;

namespace SimpleTester.UnitTests;

public class TestRunOperationBuilderTests
{
    [Fact]
    public void UseOperation_InvalidReturnType_ShouldThrow()
    {
        // Arrange
        var useOperationFunc = () => TestSuite.Setup().UseOperation<string>(StaticMethods.Adder);

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