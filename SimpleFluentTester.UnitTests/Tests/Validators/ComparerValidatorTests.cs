using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

// TODO add tests for parameters comparer
public sealed class ComparerValidatorTests
{
    [Fact]
    public void ComparerValidator_ComparableTypeWithoutComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1], new EquatableTestObject(1));
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_NotComparableTypeWithoutComparer_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1], new NotEquatableTestObject(1));
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, $"{typeof(NotEquatableTestObject).FullName} type should be assignable from IEquatable`1 or comparer should be defined");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithValidComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1], 1);
        container.WithComparer((int x, int y) => x == y);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithComparer_ParameterTypesDifferentFromExpected_ShouldBeInvalid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1], new NotEquatableTestObject(1));
        container.WithComparer((int x, int y) => x == y);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertInvalid(ValidationSubject.Comparer, $"Test case type was {typeof(NotEquatableTestObject).FullName}, but comparer type is System.Int32");
    }
}