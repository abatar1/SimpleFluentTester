using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class OperationValidatorTests
{
    [Fact]
    public void OperationValidator_NotValidValidatedObjectType_ThrowException()
    {
        // Assign
        var validator = new OperationValidator();
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());

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
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue([], 1);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Operation, "Operation not specified");
    }
    
    [Fact]
    public void OperationValidator_DelegateWithoutReturn_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation(() => { });
        var testCase = container.DefineTestCaseWithExpectedValue([], 1);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Operation, "Operation must have return type to be testable");
    }
    
    [Fact]
    public void OperationValidator_ExceptionTestCase_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue([], new Exception());

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableReturnTypeNullExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int? _) => (int?)null);
        var testCase = container.DefineTestCaseWithExpectedValue([], null);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableIntReturnTypeIntExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int? _) => 1);
        var testCase = container.DefineTestCaseWithExpectedValue([], 1);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_NullableIntReturnTypeStringExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int? _) => (int?) null);
        var testCase = container.DefineTestCaseWithExpectedValue([], "test");

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
    }
    
    [Fact]
    public void OperationValidator_IntReturnTypeStringExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int? _) => (int?) null);
        var testCase = container.DefineTestCaseWithExpectedValue([], "test");

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
    }
    
    [Fact]
    public void OperationValidator_IntReturnTypeIntExpected_ShouldBeValid()
    {
        // Assign
        var validator = new OperationValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue([], 1);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void OperationValidator_InvalidValidatedType_ShouldThrow()
    {
        // Assign
        var validator = new OperationValidator();
        var customValidated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());

        // Act
        var func = () => validator.Validate(customValidated, new EmptyValidationContext());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void OperationValidator_OperationThrowsExceptionButExpectParameter_ShouldBeAlwaysValid()
    {
        // Assign
        var validator = new InputsValidator();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((Func<int?, int?, int>)((_, _) => throw new Exception()));
        
        var parameter = ExpectParameterFactory.Create(container, 0);
        var clauses = TestClauseFactory.DefineFromParameter(1, parameter);
        var testCase = container.DefineTestCase([1, 1], clauses);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
}