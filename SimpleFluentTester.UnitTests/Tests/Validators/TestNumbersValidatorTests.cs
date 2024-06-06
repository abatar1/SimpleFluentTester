using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class TestNumbersValidatorTests
{
    [Fact]
    public void TestNumbersValidator_NotValidValidatedObjectType_ThrowException()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        
        // Act
        var func = () => validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void TestNumbersValidator_NotValidValidatorType_ThrowException()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        var validationContext = new TestNumbersValidationContext(new HashSet<int>());
        
        // Act
        var func = () => validator.Validate(validated, validationContext);

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void TestNumbersValidator_EmptyNumbersSet_ShouldBeValid()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var validationContext = new TestNumbersValidationContext(new HashSet<int>());
        
        // Act
        var validationResult = validator.Validate(container.Context, validationContext);

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void TestNumbersValidator_NumbersSetIsValid_ShouldBeValid()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var validationContext = new TestNumbersValidationContext(new HashSet<int>([1]));
        
        TestSuiteFactory.CreateAndAddTestCase(container, [1], "test");
        
        // Act
        var validationResult = validator.Validate(container.Context, validationContext);

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void TestNumbersValidator_NumbersSetIsInvalid_ShouldBeInvalid()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var validationContext = new TestNumbersValidationContext(new HashSet<int>([2]));
        
        TestSuiteFactory.CreateAndAddTestCase(container, [1], "test");
        
        // Act
        var validationResult = validator.Validate(container.Context, validationContext);

        // Assert
        validationResult.AssertInvalid(ValidationSubject.TestNumbers, "Invalid test case numbers were given as input");
    }
    
    [Fact]
    public void TestNumbersValidator_NumbersCountIsInvalid_ShouldBeInvalid()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var validationContext = new TestNumbersValidationContext(new HashSet<int>([1, 2]));
        
        TestSuiteFactory.CreateAndAddTestCase(container, [1], "test");
        
        // Act
        var validationResult = validator.Validate(container.Context, validationContext);

        // Assert
        validationResult.AssertInvalid(ValidationSubject.TestNumbers, "Invalid test case numbers were given as input");
    }
}