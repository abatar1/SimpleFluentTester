using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests;

public class ValidatorTests
{
    [Fact]
    public void OperationValidator_NotValidValidatorType_ThrowException()
    {
        // Assign
        var operationValidator = new OperationValidator();
        var context = TestHelpers.CreateEmptyContext<object>();
        
        // Act
        var func = () => operationValidator.Validate(context, new EmptyValidatedObject());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void InputsValidator_NotValidValidatorType_ThrowException()
    {
        // Assign
        var operationValidator = new InputsValidator();
        var context = TestHelpers.CreateEmptyContext<object>();
        
        // Act
        var func = () => operationValidator.Validate(context, new EmptyValidatedObject());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void TestNumbersValidator_NotValidValidatorType_ThrowException()
    {
        // Assign
        var operationValidator = new TestNumbersValidator();
        var context = TestHelpers.CreateEmptyContext<object>();
        
        // Act
        var func = () => operationValidator.Validate(context, new EmptyValidatedObject());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
}