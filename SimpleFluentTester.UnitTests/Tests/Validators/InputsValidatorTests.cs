using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class InputsValidatorTests
{
    [Fact]
    public void InputsValidator_NotValidValidatedObjectType_ThrowException()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        
        // Act
        var func = () => validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void InputsValidator_InvalidValidatedType_ShouldThrow()
    {
        // Assign
        var validator = new InputsValidator();
        var customValidated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        var validatedObject = new InputsValidatedObject(() => true);
        
        // Act
        var func = () => validator.Validate(customValidated, validatedObject);

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void InputsValidator_InvalidInputNumbers_ShouldBeInvalid()
    {
        // Assign
        var validator = new InputsValidator();
        var testCase = TestSuiteFactory.CreateTestCase([1], "test");
        var validatedObject = new InputsValidatedObject(() => true);
        
        // Act
        var validationResult = validator.Validate(testCase, validatedObject);

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Inputs, "Invalid inputs number, should be 0, but was 1, inputs 1");
    }
    
    [Fact]
    public void InputsValidator_ValidNumberInvalidTypes_ShouldBeInvalid()
    {
        // Assign
        var validator = new InputsValidator();
        var testCase = TestSuiteFactory.CreateTestCase(["test"], "test");
        var validatedObject = new InputsValidatedObject((int _) => true);
        
        // Act
        var validationResult = validator.Validate(testCase, validatedObject);

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Inputs, "Passed parameters and expected operation parameters are not equal");
    }
    
    [Fact]
    public void InputsValidator_StringTestCaseWithNullableParameter_ShouldBeInvalid()
    {
        // Assign
        var validator = new InputsValidator();
        var testCase = TestSuiteFactory.CreateTestCase(["test"], "test");
        var validatedObject = new InputsValidatedObject((int? _) => true);
        
        // Act
        var validationResult = validator.Validate(testCase, validatedObject);

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Inputs, "Passed parameters and expected operation parameters are not equal");
    }
    
    [Fact]
    public void InputsValidator_NullTestCaseWithNullableParameter_ShouldBeValid()
    {
        // Assign
        var validator = new InputsValidator();
        var testCase = TestSuiteFactory.CreateTestCase([null], "test");
        var validatedObject = new InputsValidatedObject((int? _) => true);
        
        // Act
        var validationResult = validator.Validate(testCase, validatedObject);

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void InputsValidator_IntTestCaseWithIntParameter_ShouldBeValid()
    {
        // Assign
        var validator = new InputsValidator();
        var testCase = TestSuiteFactory.CreateTestCase([1], "test");
        var validatedObject = new InputsValidatedObject((int _) => true);
        
        // Act
        var validationResult = validator.Validate(testCase, validatedObject);

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void InputsValidator_IntTestCaseWithNullableIntParameter_ShouldBeValid()
    {
        // Assign
        var validator = new InputsValidator();
        var testCase = TestSuiteFactory.CreateTestCase([1], "test");
        var validatedObject = new InputsValidatedObject((int? _) => true);
        
        // Act
        var validationResult = validator.Validate(testCase, validatedObject);

        // Assert
        validationResult.AssertValid();
    }
}