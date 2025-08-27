using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

// TODO add tests for parameters inputs
public sealed class InputsValidatorTests
{
    [Fact]
    public void InputsValidator_InvalidValidatedType_ShouldThrow()
    {
        // Assign
        var validator = new InputsValidator();
        var customValidated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
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
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        var testCase = container.DefineTestCaseWithExpectedValue([1], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Inputs, "Invalid inputs number, should have 2 parameters, but had 1: [1]");
    }
    
    [Fact]
    public void InputsValidator_ValidNumberInvalidTypes_ShouldBeInvalid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue(["test"], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Inputs, "Passed parameters and expected operation parameters are not equal.");
    }
    
    [Fact]
    public void InputsValidator_StringTestCaseWithNullableParameter_ShouldBeInvalid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int? x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue(["test"], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Inputs, "Passed parameters and expected operation parameters are not equal.");
    }
    
    [Fact]
    public void InputsValidator_NullTestCaseWithNullableParameter_ShouldBeValid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int? x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue([null], string.Empty);
        
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
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        var testCase = container.DefineTestCaseWithExpectedValue([1, 1], string.Empty);
        
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
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int? x, int? y) => x + y);
        var testCase = container.DefineTestCaseWithExpectedValue([1, 1], string.Empty);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void InputsValidator_ExpectedValidParameter_ShouldBeValid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int? x, int? y) => x + y);
        
        var parameter = ExpectParameterFactory.Create(container, 0);
        var clauses = TestClauseFactory.DefineFromParameter(1, parameter);
        var testCase = container.DefineTestCase([1, 1], clauses);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void InputsValidator_ExpectedParameterFromDifferentOperation_ShouldBeNonValid()
    {
        // Assign
        var validator = new InputsValidator();
        
        var container1 = TestSuiteContextContainer.Default();
        container1.WithOperation((int? x, int? y) => x + y);
        
        var container2 = TestSuiteContextContainer.Default();
        container2.WithOperation((int? x, int? y) => x + y);
        
        var parameter = ExpectParameterFactory.Create(container1, 1);
        var clauses = TestClauseFactory.DefineFromParameter(1, parameter);
        var testCase = container2.DefineTestCase([1, 1], clauses);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Inputs, "Could not find parameter with name y and position 1.");
    }
}