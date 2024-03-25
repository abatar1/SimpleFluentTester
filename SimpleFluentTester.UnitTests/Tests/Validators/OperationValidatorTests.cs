using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class OperationValidatorTests
{
    [Fact]
    public void OperationValidator_NotValidValidatedObjectType_ThrowException()
    {
        // Assign
        var validator = new OperationValidator();
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());

        // Act
        var func = () => validator.Validate(validated, new EmptyValidatedObject());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void OperationValidator_NullDelegate_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var validatedObject = new OperationValidatedObject(null);
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());

        // Act
        var validatedResult = validator.Validate(validated, validatedObject);

        // Assert
        validatedResult.AssertInvalid(ValidationSubject.Operation, "Operation not specified");
    }
    
    [Fact]
    public void OperationValidator_DelegateWithoutReturn_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        var validatedObject = new OperationValidatedObject(() => { });

        // Act
        var validatedResult = validator.Validate(validated, validatedObject);

        // Assert
        validatedResult.AssertInvalid(ValidationSubject.Operation, "Operation must have return type to be testable");
    }
    
    [Fact]
    public void OperationValidator_ExceptionTestCase_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var testCase = TestSuiteTestFactory.CreateTestCase([1], new Exception());
        var validatedObject = new OperationValidatedObject(() => true);

        // Act
        var validatedResult = validator.Validate(testCase, validatedObject);

        // Assert
        validatedResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableReturnTypeNullExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var testCase = TestSuiteTestFactory.CreateTestCase([1], null);
        var validatedObject = new OperationValidatedObject((int? x) => x);

        // Act
        var validatedResult = validator.Validate(testCase, validatedObject);

        // Assert
        validatedResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableIntReturnTypeIntExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var testCase = TestSuiteTestFactory.CreateTestCase([1], 1);
        var validatedObject = new OperationValidatedObject((int? x) => x);

        // Act
        var validatedResult = validator.Validate(testCase, validatedObject);

        // Assert
        validatedResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableIntReturnTypeStringExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var testCase = TestSuiteTestFactory.CreateTestCase([1], "test");
        var validatedObject = new OperationValidatedObject((int? x) => x);

        // Act
        var validatedResult = validator.Validate(testCase, validatedObject);

        // Assert
        validatedResult.AssertInvalid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
    }
    
    [Fact]
    public void OperationValidator_IntReturnTypeStringExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var testCase = TestSuiteTestFactory.CreateTestCase([1], "test");
        var validatedObject = new OperationValidatedObject((int x) => x);

        // Act
        var validatedResult = validator.Validate(testCase, validatedObject);

        // Assert
        validatedResult.AssertInvalid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
    }
    
    [Fact]
    public void OperationValidator_IntReturnTypeIntExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var testCase = TestSuiteTestFactory.CreateTestCase([1], 1);
        var validatedObject = new OperationValidatedObject((int x) => x);

        // Act
        var validatedResult = validator.Validate(testCase, validatedObject);

        // Assert
        validatedResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_InvalidValidatedType_ShouldThrow()
    {
        // Assign
        var validator = new OperationValidator();
        var customValidated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        var validatedObject = new OperationValidatedObject(() => true);

        // Act
        var func = () => validator.Validate(customValidated, validatedObject);

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
}