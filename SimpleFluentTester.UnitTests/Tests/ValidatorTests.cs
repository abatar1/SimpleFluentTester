using SimpleFluentTester.TestSuite.Context;
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

    [Fact]
    public void BaseValidator_CheckEquality_ShouldBeValid()
    {
        // Assign
        const string key = "Key";
        var testValidator1 = new TestValidator(key, ValidationSubject.Operation);
        var testValidator2 = new TestValidator(key, ValidationSubject.Operation);

        // Act
        var isEqual1 = testValidator1.Equals(testValidator2);
        var isEqual2 = Equals(testValidator1, testValidator2);
        var hashset = new HashSet<TestValidator> { testValidator1, testValidator2 };

        // Assert        
        Assert.True(isEqual1);
        Assert.True(isEqual2);
        Assert.Single(hashset);
    }

    private sealed class TestValidator(string key, ValidationSubject validationSubject) : BaseValidator
    {
        public override string Key => key;

        public override ValidationResult Validate<TOutput>(ITestSuiteBuilderContext<TOutput> context, IValidatedObject validatedObject)
        {
            return new ValidationResult(ValidationStatus.Valid, validationSubject);
        }
    }
}