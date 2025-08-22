using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators;
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
        var validator = new CustomValidator(subject);

        // Assert
        Assert.Equal(nameof(CustomValidator), validator.Key);
        Assert.Equal(subject, validator.Subject);
    }
    
    [Fact]
    public void AddValidation_AddSingleValidation_ShouldBeSingle()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
        // Act
        validated.AddReadyValidation(ValidationTestResults.Valid);

        // Assert
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
        Assert.Single(validated.Validations);
        Assert.Equal(2, validated.Validations.Values.First().Count);
    }
    
    [Fact]
    public void RegisterValidation_InvalidValidator_ShouldThrowException()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());
        
        // Act
        var func = () => validated.RegisterFutureValidation<CustomValidator>();

        // Assert
        Assert.Throws<InvalidOperationException>(func);
    }
    
    [Fact]
    public void RegisterValidation_ValidRegistration_ShouldBeValid()
    {
        // Assign
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        container.WithOperation((int x, int y) => x + y);
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1], 1);
        testCase.RegisterFutureValidation<OperationValidator>();
        
        // Act
        var validated = testCase.Validate();

        // Assert
        Assert.Equal(ValidationStatus.Valid, validated);
        testCase.AssertValid();
    }
}