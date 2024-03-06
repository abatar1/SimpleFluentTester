using SimpleFluentTester.Suite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests
{
    public class TestRunOperationBuilderTests
    {
        [Fact]
        public void UseOperation_InvalidReturnType_ShouldThrow()
        {
            // Arrange
            var setup = TestSuite.Sequential.WithExpectedReturnType<string>();
        
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
            var setup = TestSuite.Sequential.WithExpectedReturnType<int>();
        
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
            var setup = TestSuite.Sequential.WithExpectedReturnType<int>();
        
            // Act
            var reporter = setup.UseOperation(StaticMethods.Adder).Run();

            // Assert
            var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
            var validationResult = testRunResult.ContextValidationResults
                .SingleOrDefault(x => x.ValidationSubject == ValidationSubject.Operation);
        
            Assert.NotNull(validationResult);
            Assert.True(validationResult.IsValid);
            Assert.Equal(ValidationSubject.Operation, validationResult.ValidationSubject);
            Assert.Null(validationResult.Message);
        }
    }
}