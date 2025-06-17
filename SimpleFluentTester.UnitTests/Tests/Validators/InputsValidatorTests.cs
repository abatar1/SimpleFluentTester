using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

// TODO add tests for parameters inputs
public sealed class InputsValidatorTests
{
    [Fact]
    public void InputsValidator_InvalidValidatedType_ShouldThrow()
    {
        // Assign
        var validator = new InputsValidator();
        var customValidated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>>());
        
        // Act
        var func = () => validator.Validate(customValidated, new EmptyValidationContext());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void InputsValidator_InvalidInputParametersCount_ShouldBeInvalid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int x, int y) => x + y);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Inputs, "Invalid inputs number, should have 2 parameters, but had 1: [1]");
    }
    
    [Fact]
    public void InputsValidator_ValidNumberInvalidTypes_ShouldBeInvalid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int x) => x);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, ["test"], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Inputs, "Passed parameters and expected operation parameters are not equal.");
    }
    
    [Fact]
    public void InputsValidator_StringTestCaseWithNullableParameter_ShouldBeInvalid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int? x) => x);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, ["test"], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Inputs, "Passed parameters and expected operation parameters are not equal.");
    }
    
    [Fact]
    public void InputsValidator_NullTestCaseWithNullableParameter_ShouldBeValid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int? x) => x);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [null], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void InputsValidator_IntTestCaseWithIntParameter_ShouldBeValid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int x, int y) => x + y);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1, 1], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void InputsValidator_IntTestCaseWithNullableIntParameter_ShouldBeValid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int? x, int? y) => x + y);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1, 1], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
}