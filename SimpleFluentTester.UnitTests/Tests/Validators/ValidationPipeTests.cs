using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class ValidationPipeTests
{
    [Fact]
    public void ValidatePacked_EmptyValidations_ShouldBeValid()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>>());

        // Act
        var validationResult = validated.Validate();

        // Assert
        Assert.Equal(ValidationStatus.Valid, validationResult);
        validated.AssertValid();
    }
    
    [Fact]
    public void ValidatePacked_ValidValidation_ShouldBeValid()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        var validationResults = new List<Lazy<ValidationResult>> { new(() => ValidationResult.Valid(subject)) };
        var validations = new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var validationResult = validated.Validate();

        // Assert
        Assert.Equal(ValidationStatus.Valid, validationResult);
        validated.AssertValid();
    }
    
    [Fact]
    public void ValidatePacked_NonValidValidation_ShouldBeNonValid()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message = "ErrorMessage";
        var validationResults = new List<Lazy<ValidationResult>>
        {
            new(() => ValidationResult.NonValid(subject, message)),
            new(() => ValidationResult.Valid(subject))
        };
        var validations = new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var validationResult = validated.Validate();
        
        // Assert
        Assert.Equal(ValidationStatus.NonValid, validationResult);
        validated.AssertNonValid(subject, message);
    }
    
    [Fact]
    public void ValidatePacked_MultipleNonValidValidation_ShouldBeNonValidAndAggregated()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message1 = "ErrorMessage1";
        const string message2 = "ErrorMessage2";
        var validationResults = new List<Lazy<ValidationResult>>
        {
            new(() => ValidationResult.NonValid(subject, message1)),
            new(() => ValidationResult.NonValid(subject, message2))
        };
        var validations = new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var validationResult = validated.Validate();

        // Assert
        Assert.Equal(ValidationStatus.NonValid, validationResult);
        validated.AssertNonValid(subject, message1, message2);
    }
    
    [Fact]
    public void ValidatePacked_NonValidAndFailedValidation_ShouldBeFailed()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message1 = "ErrorMessage1";
        const string message2 = "ErrorMessage2";
        var exception = new InvalidDataException("Exception1");

        var validationResults = new List<Lazy<ValidationResult>>
        {
            new(() => ValidationResult.NonValid(subject, message1)),
            new(() => ValidationResult.Failed(subject, exception, message2))
        };
        var validations = new Dictionary<ValidationSubject, IList<Lazy<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var validationResult = validated.Validate();

        // Assert
        Assert.Equal(ValidationStatus.Failed, validationResult);
        Assert.Contains(validated.Validations[subject], x => x.Value is { Status: ValidationStatus.NonValid, Message: message1 });
        Assert.Contains(validated.Validations[subject], x => x.Value is { Status: ValidationStatus.Failed, Message: message2 });
    }
}