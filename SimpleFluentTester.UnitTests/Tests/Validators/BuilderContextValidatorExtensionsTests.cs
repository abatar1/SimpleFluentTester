using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators.Helpers;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class BuilderContextValidatorExtensionsTests
{
    [Fact]
    public void BaseValidator_Initialize_ShouldBeValid()
    {
        // Assign
        var subject = ValidationSubject.Operation;
        
        // Act
        var validator = new EmptyValidator(subject);

        // Assert
        Assert.Equal(nameof(EmptyValidator), validator.Key);
        Assert.Equal(subject, validator.Subject);
    }
    
    [Fact]
    public void AddValidation_AddInvalidAndValidValidations_ShouldInvalid()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
        // Act
        validated.AddReadyValidation(ValidationTestResults.NonValid);
        validated.AddReadyValidation(ValidationTestResults.Valid);

        // Assert
        Assert.False(validated.IsValid());
        Assert.Single(validated.Validations);
    }
    
    [Fact]
    public void AddValidation_AddFailedValidation_ShouldInvalid()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
        // Act
        validated.AddReadyValidation(ValidationTestResults.Failed);
        validated.AddReadyValidation(ValidationTestResults.Valid);

        // Assert
        Assert.False(validated.IsValid());
        Assert.Single(validated.Validations);
    }
    
    [Fact]
    public void AddValidation_AddSingleInvalidValidation_ShouldBeSingleAndInvalid()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
        // Act
        validated.AddReadyValidation(ValidationTestResults.NonValid);

        // Assert
        Assert.False(validated.IsValid());
        Assert.Single(validated.Validations);
        Assert.Single(validated.Validations.Values.First());
    }
    
    [Fact]
    public void AddValidation_AddValidValidation_ShouldBeSingleAndValid()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
        // Act
        validated.AddReadyValidation(ValidationTestResults.Valid);

        // Assert
        Assert.True(validated.IsValid());
        Assert.Single(validated.Validations);
        Assert.Single(validated.Validations.Values.First());
    }
    
    [Fact]
    public void AddValidation_AddTwoValidations_ShouldBeValid()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
        // Act
        validated.AddReadyValidation(ValidationTestResults.Valid);
        validated.AddReadyValidation(ValidationTestResults.Valid);

        // Assert
        Assert.True(validated.IsValid());
        Assert.Single(validated.Validations);
        Assert.Equal(2, validated.Validations.Values.First().Count);
    }
    
    [Fact]
    public void RegisterValidation_InvalidValidator_ShouldThrowException()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
        // Act
        var func = () => validated.RegisterFutureValidation<EmptyValidator>();

        // Assert
        Assert.Throws<InvalidOperationException>(func);
    }
    
    [Fact]
    public void RegisterValidation_ValidRegistration_ShouldBeValid()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        var testCase = container.DefineTestCaseWithExpectedValue([1], 1);
        testCase.RegisterFutureValidation<OkTestValidator>(args: [ValidationSubject.Expect]);
        
        // Act
        _ = testCase.Validate();

        // Assert
        var validations = testCase.Validations
            .FirstOrDefault(x => x.Key == ValidationSubject.Expect).Value;
        Assert.Single(validations);
        validations.First().Value.AssertValid();
    }
}