using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Helpers;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class ValidationPipeTests
{
    [Fact]
    public void ValidatePacked_EmptyValidations_ShouldBeValid()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>());

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
        
        var validationResults = PrepareSubjectValidation(ValidationResult.Valid(subject));
        
        var validations = new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> { { subject, validationResults } };
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
        
        var validationResults = PrepareSubjectValidation(ValidationResult.NonValid(subject, message), ValidationResult.Valid(subject));
        
        var validations = new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> { { subject, validationResults } };
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
        const ValidationSubject subject1 = ValidationSubject.Operation;
        const string message1 = "ErrorMessage1";
        const ValidationSubject subject2 = ValidationSubject.Comparer;
        const string message2 = "ErrorMessage2";
        
        var validations = new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>
        {
            { subject1, PrepareSubjectValidation(ValidationResult.NonValid(subject1, message1)) },
            { subject2, PrepareSubjectValidation(ValidationResult.NonValid(subject2, message2)) },
        };
        var validated = new CustomValidatedObject(validations);

        // Act
        var validationResult = validated.Validate();

        // Assert
        Assert.Equal(ValidationStatus.NonValid, validationResult);
        validated.AssertNonValid(subject1, message1);
        validated.AssertNonValid(subject2, message2);
    }
    
    [Fact]
    public void ValidatePacked_NonValidAndFailedValidation_ShouldBeFailed()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message1 = "ErrorMessage1";
        const string message2 = "ErrorMessage2";
        var exception = new InvalidDataException("Exception1");

        var validationResults = PrepareSubjectValidation(ValidationResult.NonValid(subject, message1), ValidationResult.Failed(subject, exception, message2));
    
        var validations = new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var validationResult = validated.Validate();

        // Assert
        Assert.Equal(ValidationStatus.Failed, validationResult);
        Assert.Contains(validated.Validations[subject], x => x.Value.Validations.Any(y => y is { Status: ValidationStatus.NonValid, Message: message1 }));
        Assert.Contains(validated.Validations[subject], x => x.Value.Validations.Any(y => y is { Status: ValidationStatus.Failed, Message: message2 }));
    }

    private List<Lazy<SubjectValidation>> PrepareSubjectValidation(params ValidationResult[] results)
    {
        return [new(() => new SubjectValidation(results))];
    }
}