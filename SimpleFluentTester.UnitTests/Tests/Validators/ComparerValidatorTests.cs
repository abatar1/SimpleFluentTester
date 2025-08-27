using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class ComparerValidatorTests
{
    [Fact]
    public void ComparerValidator_ComparableTypeWithoutComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue([1], new EquatableTestObject(1));
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_ComparableArrayTypeWithoutComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue([new[] {1, 2, 3}], new[] {1, 2, 3});
        
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
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue([1], new NotEquatableTestObject(1));
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Comparer, $"{typeof(NotEquatableTestObject).FullName} type should be assignable from IEquatable`1 or comparer should be defined");
    }
    
    [Fact]
    public void ComparerValidator_ComparableTypeWithValidComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue([1], 1);
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
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue([1], new NotEquatableTestObject(1));
        container.WithComparer((int x, int y) => x == y);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Comparer, $"Test case type was {typeof(NotEquatableTestObject).FullName}, but comparer type is System.Int32");
    }
    
    [Fact]
    public void ComparerValidator_NotEquatableTestObjectWithComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue( [new NotEquatableTestObject(1)], new NotEquatableTestObject(1));
        container.WithComparer((NotEquatableTestObject? x, NotEquatableTestObject? y) => x?.Value == y?.Value);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_CompareEqualIntSequencesWithDefinedComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue([new[] {1, 2, 3}, new[] {1, 2, 3}], new[] {1, 2, 3});
        container.WithComparer((int[]? x, int[]? y) => x?.SequenceEqual(y ?? []) ?? y == null);

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_CompareEqualIntSequencesWithoutDefinedComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue([new[] {1, 2, 3}, new[] {1, 2, 3}], new[] {1, 2, 3});

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
    
    [Fact]
    public void ComparerValidator_CompareEqualSequencesWithoutDefinedComparer_ShouldBeNonValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue(
            [
                new[] { new NotEquatableTestObject(1) },
                new[] { new NotEquatableTestObject(1) }
            ],
            new[] { new NotEquatableTestObject(1) });

        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertNonValid(ValidationSubject.Comparer, "SimpleFluentTester.UnitTests.Helpers.TestObjects.NotEquatableTestObject elements should implement IEquatable`1 or a comparer should be defined");
    }
    
    [Fact]
    public void ComparerValidator_CompareEqualSequencesWithDefinedComparer_ShouldBeValid()
    {
        // Assign
        var validator = new ComparerValidator();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue(
            [
                new[] { new NotEquatableTestObject(1) },
                new[] { new NotEquatableTestObject(1) }
            ],
            new[] { new NotEquatableTestObject(1) });
        container.WithComparer((NotEquatableTestObject? x, NotEquatableTestObject? y) => x?.Value == y?.Value);
        
        // Act
        var validationResult = validator.Validate(testCase, new EmptyValidationContext());

        // Assert
        validationResult.AssertValid();
    }
}