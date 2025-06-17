using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

// TODO add tests for parameters operation
public sealed class OperationValidatorTests
{
    [Fact]
    public void OperationValidator_NotValidValidatedObjectType_ThrowException()
    {
        // Assign
        var validator = new OperationValidator();
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>>());

        // Act
        var func = () => validator.Validate(validated, new EmptyValidationContext());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void OperationValidator_NullDelegate_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [], 1);

        // Act
        var validatedResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validatedResult.AssertInvalid(ValidationSubject.Operation, "Operation not specified");
    }
    
    [Fact]
    public void OperationValidator_DelegateWithoutReturn_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation(() => { });
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [], 1);

        // Act
        var validatedResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validatedResult.AssertInvalid(ValidationSubject.Operation, "Operation must have return type to be testable");
    }
    
    [Fact]
    public void OperationValidator_ExceptionTestCase_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int x) => x);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [], new Exception());

        // Act
        var validatedResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validatedResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableReturnTypeNullExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int? x) => (int?)null);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [], null);

        // Act
        var validatedResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validatedResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableIntReturnTypeIntExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int? x) => 1);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [], 1);

        // Act
        var validatedResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validatedResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableIntReturnTypeStringExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int? x) => (int?) null);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [], "test");

        // Act
        var validatedResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validatedResult.AssertInvalid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
    }
    
    [Fact]
    public void OperationValidator_IntReturnTypeStringExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int? x) => (int?) null);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [], "test");

        // Act
        var validatedResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validatedResult.AssertInvalid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
    }
    
    [Fact]
    public void OperationValidator_IntReturnTypeIntExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int x) => x);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [], 1);

        // Act
        var validatedResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validatedResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_InvalidValidatedType_ShouldThrow()
    {
        // Assign
        var validator = new OperationValidator();
        var customValidated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>>());

        // Act
        var func = () => validator.Validate(customValidated, new EmptyValidationContext());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
}