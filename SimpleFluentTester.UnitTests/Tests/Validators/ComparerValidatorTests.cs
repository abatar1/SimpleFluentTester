using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class ComparerValidatorTests
{
    [Fact]
    public void ComparerValidator_InvalidValidatedType_ShouldThrow()
    {
        // Assign
        var validator = new ComparerValidator();
        var customValidated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        
        // Act
        var func = () => validator.Validate(customValidated, new EmptyValidationContext());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void ComparerValidator_EmptyContext_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        
        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_MoreThanOneExpectedTypes_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();

        TestSuiteFactory.CreateAndAddTestCase(container, [1], "test");
        
        TestSuiteFactory.CreateAndAddTestCase(container, [1], 13);
        
        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, "TestCase expected object types are more than one in TestSuite collection");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithoutComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        
        TestSuiteFactory.CreateAndAddTestCase(container, [1], new EquatableTestObject(1));
        
        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_NotComparableTypeWithoutComparer_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        
        TestSuiteFactory.CreateAndAddTestCase(container, [1], new NotEquatableTestObject(1));
        
        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, $"{typeof(NotEquatableTestObject).FullName} type should be assignable from IEquatable`1 or comparer should be defined");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithValidComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();

        TestSuiteFactory.CreateAndAddTestCase(container, [1], 1);

        container.WithComparer((int x, int y) => x == y);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_NotBooleanReturn_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();

        TestSuiteFactory.CreateAndAddTestCase(container, [1], 1);

        container.WithComparer((int x, int y) => x + y);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, "Return type of the custom comparer is not bool but System.Int32, something went wrong during initialization");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_MoreThanTwoParameters_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();

        TestSuiteFactory.CreateAndAddTestCase(container, [1], 1);

        container.WithComparer((int _, int _, int _) => true);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, "Custom comparer has 3 parameters, but should has 2, something went wrong during initialization");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_ParameterTypesDifferent_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();

        TestSuiteFactory.CreateAndAddTestCase(container, [1], 1);

        container.WithComparer((int _, string _) => true);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, "Comparer has not the same input parameter type, something went wrong during initialization");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_ParameterTypesDifferentFromExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();

        TestSuiteFactory.CreateAndAddTestCase(container, [1], new NotEquatableTestObject(1));

        container.WithComparer((int x, int y) => x == y);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, $"Test case type was {typeof(NotEquatableTestObject).FullName}, but comparer type is System.Int32");
    }
}