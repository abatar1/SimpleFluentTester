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
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();
        
        // Act
        var func = () => validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void TestNumbersValidator_NotValidValidatorType_ThrowException()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        var validatedObject = new TestNumbersValidatedObject(new HashSet<int>());
        
        // Act
        var func = () => validator.Validate(validated, validatedObject);

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void TestNumbersValidator_EmptyNumbersSet_ShouldBeValid()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();
        var validatedObject = new TestNumbersValidatedObject(new HashSet<int>());
        
        // Act
        var validationResult = validator.Validate(container.Context, validatedObject);

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void TestNumbersValidator_NumbersSetIsValid_ShouldBeValid()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();
        var validatedObject = new TestNumbersValidatedObject(new HashSet<int>([1]));
        
        var testCase1 = TestSuiteTestFactory.CreateTestCase([1], "test");
        container.Context.TestCases.Add(testCase1);
        
        // Act
        var validationResult = validator.Validate(container.Context, validatedObject);

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void TestNumbersValidator_NumbersSetIsInvalid_ShouldBeInvalid()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();
        var validatedObject = new TestNumbersValidatedObject(new HashSet<int>([2]));
        
        var testCase1 = TestSuiteTestFactory.CreateTestCase([1], "test");
        container.Context.TestCases.Add(testCase1);
        
        // Act
        var validationResult = validator.Validate(container.Context, validatedObject);

        // Assert
        validationResult.AssertInvalid(ValidationSubject.TestNumbers, "Invalid test case numbers were given as input");
    }
    
    [Fact]
    public void TestNumbersValidator_NumbersCountIsInvalid_ShouldBeInvalid()
    {
        // Assign
        var validator = new TestNumbersValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();
        var validatedObject = new TestNumbersValidatedObject(new HashSet<int>([1, 2]));
        
        var testCase1 = TestSuiteTestFactory.CreateTestCase([1], "test");
        container.Context.TestCases.Add(testCase1);
        
        // Act
        var validationResult = validator.Validate(container.Context, validatedObject);

        // Assert
        validationResult.AssertInvalid(ValidationSubject.TestNumbers, "Invalid test case numbers were given as input");
    }
}