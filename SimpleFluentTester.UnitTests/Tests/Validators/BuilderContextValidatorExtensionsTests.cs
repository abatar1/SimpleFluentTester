using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.Validators;

public class BuilderContextValidatorExtensionsTests
{
    [Fact]
    public void BaseValidator_Initialize_ShouldBeValid()
    {
        // Assign
        var subject = ValidationSubject.Operation;
        
        // Act
        IValidator validator = new CustomValidator(subject);

        // Assert
        Assert.Equal(nameof(CustomValidator), validator.Key);
        Assert.Equal(subject, validator.Subject);
        Assert.NotEmpty(validator.AllowedTypes);
    }
    
    [Fact]
    public void AddValidation_AddSingleValidation_ShouldBeSingle()
    {
        // Assign
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        
        // Act
        validated.AddValidation(ValidationTestResults.Valid);

        // Assert
        Assert.Single(validated.Validations);
        Assert.Single(validated.Validations.Values.First());
    }
    
    [Fact]
    public void AddValidation_AddTwoValidations_ShouldBeValid()
    {
        // Assign
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        
        // Act
        validated.AddValidation(ValidationTestResults.Valid);
        validated.AddValidation(ValidationTestResults.Valid);

        // Assert
        Assert.Single(validated.Validations);
        Assert.Equal(2, validated.Validations.Values.First().Count);
    }
    
    [Fact]
    public void RegisterValidation_InvalidValidator_ShouldThrowException()
    {
        // Assign
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        
        // Act
        var func = () => validated.RegisterValidation<CustomValidator>();

        // Assert
        Assert.Throws<InvalidOperationException>(func);
    }
    
    [Fact]
    public void RegisterValidation_InvalidValidatedType_ShouldThrowException()
    {
        // Assign
        var validated = new CustomValidated(new Dictionary<ValidationSubject, IList<Func<ValidationResult>>>());
        
        // Act
        var func = () => validated.RegisterValidation<OperationValidator>();

        // Assert
        Assert.Throws<InvalidOperationException>(func);
    }
    
    [Fact]
    public void RegisterValidation_ValidRegistrationFailedValidator_ShouldBeInvalid()
    {
        // Assign
        var validated = TestSuiteFactory.CreateTestCase([1], 1);
        var unpacker = new ValidationUnpacker();
        
        // Act
        validated.RegisterValidation<OperationValidator>();

        // Assert
        var validation = unpacker.Unpack(validated);
        validation.AssertFailed<ValidationUnexpectedException>(ValidationSubject.Operation, "Failed to validate SimpleFluentTester.TestSuite.Case.TestCase with SimpleFluentTester.Validators.OperationValidator validator.");
    }
    
    [Fact]
    public void RegisterValidation_ValidRegistration_ShouldBeValid()
    {
        // Assign
        var validated = TestSuiteFactory.CreateTestCase([1], 1);
        var unpacker = new ValidationUnpacker();
        
        // Act
        validated.RegisterValidation<OperationValidator>(() => new OperationValidatedObject(() => 1));

        // Assert
        var validation = unpacker.Unpack(validated);
        validation.AssertValid();
    }
}