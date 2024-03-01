using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests;

public class TestRunOperationBuilderTests
{
    [Fact]
    public void UseOperation_InvalidReturnType_ShouldThrow()
    {
        // Arrange
        var setup = TestSuite.WithExpectedReturnType<string>();
        
        // Act
        var reporter = setup.UseOperation(StaticMethods.Adder).Run();

        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        var validationResult = testRunResult.ContextValidationResults
            .SingleOrDefault(x => x.ValidationSubject == ValidationSubject.Operation);
        
        Assert.NotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.Equal(ValidationSubject.Operation, validationResult.ValidationSubject);
        Assert.NotNull(validationResult.Message);
    }
    
    [Fact]
    public void UseOperation_NoReturn_ShouldThrow()
    {
        // Arrange
        var setup = TestSuite.WithExpectedReturnType<int>();
        
        // Act
        var reporter = setup.UseOperation(StaticMethods.Empty).Run();

        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        var validationResult = testRunResult.ContextValidationResults
            .SingleOrDefault(x => x.ValidationSubject == ValidationSubject.Operation);
        
        Assert.NotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.Equal(ValidationSubject.Operation, validationResult.ValidationSubject);
        Assert.NotNull(validationResult.Message);
    }
    
    [Fact]
    public void UseOperation_ValidReturnType_ShouldBeValid()
    {
        // Arrange
        var context = TestHelpers.CreateEmptyContext<int>();
        var expectedTestRunBuilder = new TestRunBuilder<int>(context);
        
        // Act
        var testRunBuilder = TestSuite.WithExpectedReturnType<int>().UseOperation(StaticMethods.Adder).Run();

        // Assert
        Assert.Equivalent(expectedTestRunBuilder, testRunBuilder, strict: true);
    }
}