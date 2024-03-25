using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public sealed class ValidationUnpackerTests
{
    [Fact]
    public void Unpack_EmptyValidations_ShouldBeValid()
    {
        // Assign
        var unpacker = new ValidationUnpacker();
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());

        // Act
        var validationUnpacked = unpacker.Unpack(validated);

        // Assert
        validationUnpacked.AssertValid();
    }
    
    [Fact]
    public void Unpack_ValidValidation_ShouldBeValid()
    {
        // Assign
        var unpacker = new ValidationUnpacker();
        const ValidationSubject subject = ValidationSubject.Operation;
        var validationResults = new List<Func<ValidationResult>> { () => ValidationResult.Valid(subject) };
        var validations = new Dictionary<ValidationSubject, IList<Func<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidated(validations);

        // Act
        var validationUnpacked = unpacker.Unpack(validated);

        // Assert
        validationUnpacked.AssertValid();
    }
    
    [Fact]
    public void Unpack_NonValidValidation_ShouldBeNonValid()
    {
        // Assign
        var unpacker = new ValidationUnpacker();
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message = "ErrorMessage";
        var validationResults = new List<Func<ValidationResult>>
        {
            () => ValidationResult.NonValid(subject, message), 
            () => ValidationResult.Valid(subject)
        };
        var validations = new Dictionary<ValidationSubject, IList<Func<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidated(validations);

        // Act
        var validationUnpacked = unpacker.Unpack(validated);

        // Assert
        validationUnpacked.AssertInvalid(subject, message);
    }
    
    [Fact]
    public void Unpack_MultipleNonValidValidation_ShouldBeNonValidAndAggregated()
    {
        // Assign
        var unpacker = new ValidationUnpacker();
        const ValidationSubject subject = ValidationSubject.Operation;
        const string message1 = "ErrorMessage1";
        const string message2 = "ErrorMessage2";
        var validationResults = new List<Func<ValidationResult>>
            { 
                () => ValidationResult.NonValid(subject, message1), 
                () => ValidationResult.NonValid(subject, message2) 
            };
        var validations = new Dictionary<ValidationSubject, IList<Func<ValidationResult>>> { { subject, validationResults } };
        var validated = new CustomValidated(validations);

        // Act
        var validationUnpacked = unpacker.Unpack(validated);

        // Assert
        validationUnpacked.AssertInvalid(subject, string.Join(Environment.NewLine, [message1, message2]));
    }
    
    [Fact]
    public void Unpack_NonValidAndFailedValidation_ShouldBeFailed()
    {
        // Assign
        var unpacker = new ValidationUnpacker();
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
        var validated = new CustomValidated(validations);

        // Act
        var validationUnpacked = unpacker.Unpack(validated);

        // Assert
        var expectedMessage = string.Join(Environment.NewLine, [message1, message2]);
        validationUnpacked.AssertFailed<InvalidDataException>(subject, expectedMessage);
    }
}