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
        var customValidated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        
        // Act
        var func = () => validator.Validate(customValidated, new EmptyValidatedObject());

        // Assert
        Assert.Throws<ValidationUnexpectedException>(func);
    }
    
    [Fact]
    public void ComparerValidator_EmptyContext_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();
        
        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_MoreThanOneExpectedTypes_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();

        var testCase1 = TestSuiteTestFactory.CreateTestCase([1], "test");
        container.Context.TestCases.Add(testCase1);
        
        var testCase2 = TestSuiteTestFactory.CreateTestCase([1], 13);
        container.Context.TestCases.Add(testCase2);
        
        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, "Expected types more than one in TestCase collection");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithoutComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();
        
        var testCase = TestSuiteTestFactory.CreateTestCase([1], new EquatableTestObject(1));
        container.Context.TestCases.Add(testCase);
        
        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_NotComparableTypeWithoutComparer_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();
        
        var testCase = TestSuiteTestFactory.CreateTestCase([1], new NotEquatableTestObject(1));
        container.Context.TestCases.Add(testCase);
        
        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, $"{typeof(NotEquatableTestObject).FullName} type should be assignable from IEquatable`1 or comparer should be defined");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithValidComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();

        var testCase = TestSuiteTestFactory.CreateTestCase([1], 1);
        container.Context.TestCases.Add(testCase);

        container.WithComparer((int x, int y) => x == y);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_NotBooleanReturn_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();

        var testCase = TestSuiteTestFactory.CreateTestCase([1], 1);
        container.Context.TestCases.Add(testCase);

        container.WithComparer((int x, int y) => x + y);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, "Return type of the custom comparer is not bool but System.Int32, something went wrong during initialization");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_MoreThanTwoParameters_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();

        var testCase = TestSuiteTestFactory.CreateTestCase([1], 1);
        container.Context.TestCases.Add(testCase);

        container.WithComparer((int _, int _, int _) => true);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, "Custom comparer has 3 parameters, but should has 2, something went wrong during initialization");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_ParameterTypesDifferent_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();

        var testCase = TestSuiteTestFactory.CreateTestCase([1], 1);
        container.Context.TestCases.Add(testCase);

        container.WithComparer((int _, string _) => true);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, "Comparer has not the same input parameter type, something went wrong during initialization");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_ParameterTypesDifferentFromExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteTestFactory.CreateEmptyContextContainer();

        var testCase = TestSuiteTestFactory.CreateTestCase([1], new NotEquatableTestObject(1));
        container.Context.TestCases.Add(testCase);

        container.WithComparer((int x, int y) => x == y);

        // Act
        var validationResult = validator.Validate(container.Context, new EmptyValidatedObject());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, $"Test case type was {typeof(NotEquatableTestObject).FullName}, but comparer type is System.Int32");
    }
}