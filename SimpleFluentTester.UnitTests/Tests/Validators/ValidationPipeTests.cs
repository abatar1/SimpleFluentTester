using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class ValidationPipeTests
{
    [Fact]
    public void ValidatePacked_EmptyValidations_ShouldBeValid()
    {
        // Assign
        var validated = new CustomValidatedObject(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());

        // Act
        var packedValidation = ValidationPipe.ValidatePacked(validated);

        // Assert
        packedValidation.AssertValid();
    }
    
    [Fact]
    public void ValidatePacked_ValidValidation_ShouldBeValid()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        var validationResults = new List<Func<ValidationResult>> { () => ValidationResult.Valid(subject) };
        var validations = new Dictionary<ValidationSubject, IList<Func<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var packedValidation = ValidationPipe.ValidatePacked(validated);

        // Assert
        packedValidation.AssertValid();
    }
    
    [Fact]
    public void ValidatePacked_NonValidValidation_ShouldBeNonValid()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message = "ErrorMessage";
        var validationResults = new List<Func<ValidationResult>>
        {
            () => ValidationResult.NonValid(subject, message), 
            () => ValidationResult.Valid(subject)
        };
        var validations = new Dictionary<ValidationSubject, IList<Func<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var packedValidation = ValidationPipe.ValidatePacked(validated);
        
        // Assert
        packedValidation.AssertInvalid(subject, message);
    }
    
    [Fact]
    public void ValidatePacked_MultipleNonValidValidation_ShouldBeNonValidAndAggregated()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message1 = "ErrorMessage1";
        const string message2 = "ErrorMessage2";
        var validationResults = new List<Func<ValidationResult>>
            { 
                () => ValidationResult.NonValid(subject, message1), 
                () => ValidationResult.NonValid(subject, message2) 
            };
        var validations = new Dictionary<ValidationSubject, IList<Func<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var packedValidation = ValidationPipe.ValidatePacked(validated);

        // Assert
        packedValidation.AssertInvalid(subject, string.Join(Environment.NewLine, [message1, message2]));
    }
    
    [Fact]
    public void ValidatePacked_NonValidAndFailedValidation_ShouldBeFailed()
    {
        // Assign
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message1 = "ErrorMessage1";
        const string message2 = "ErrorMessage2";
        var exception = new InvalidDataException("Exception1");
        
        var validationResults = new List<Func<ValidationResult>>
            { 
                () => ValidationResult.NonValid(subject, message1), 
                () => ValidationResult.Failed(subject, exception, message2),
            };
        var validations = new Dictionary<ValidationSubject, IList<Func<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidatedObject(validations);

        // Act
        var packedValidation = ValidationPipe.ValidatePacked(validated);

        // Assert
        var expectedMessage = string.Join(Environment.NewLine, [message1, message2]);
        packedValidation.AssertFailed<InvalidDataException>(subject, expectedMessage);
    }
}