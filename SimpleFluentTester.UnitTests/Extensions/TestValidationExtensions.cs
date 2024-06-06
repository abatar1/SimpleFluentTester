using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Extensions;

public static class TestValidationExtensions
{
    public static void AssertNotValidValidation(this IValidatedObject validated, ValidationSubject subject, string message)
    {
        var unpacked = ValidationPipe.ValidatePacked(validated);
        unpacked.AssertInvalid(subject, message);
    }
    
    public static void AssertValidValidation(this IValidatedObject context)
    {
        var unpacked = ValidationPipe.ValidatePacked(context);
        unpacked.AssertValid();
    }
    
    public static void AssertInvalid(this PackedValidation packedValidation, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(packedValidation);
        
        Assert.False(packedValidation.IsValid);
        var validations = packedValidation.GetNonValid();
        Assert.NotEmpty(validations);
        var validation = validations
            .FirstOrDefault(x => x.Subject == validationSubject);
        Assert.NotNull(validation);
        validation.AssertInvalid(validationSubject, message);
    }
    
    public static void AssertFailed<TException>(this PackedValidation packedValidation, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(packedValidation);
        
        Assert.False(packedValidation.IsValid);
        var validations = packedValidation.GetNonValid();
        Assert.NotEmpty(validations);
        var validation = validations
            .FirstOrDefault(x => x.Subject == validationSubject);
        Assert.NotNull(validation);
        validation.AssertFailed<TException>(validationSubject, message);
    }
    
    public static void AssertValid(this PackedValidation packedValidation)
    {
        Assert.Empty(packedValidation.GetNonValid());
        Assert.True(packedValidation.IsValid);
    }
    
    public static void AssertInvalid(this ValidationResult validationResult, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(validationResult);
        Assert.Equal(ValidationStatus.NonValid, validationResult.Status);
        Assert.Equal(validationSubject, validationResult.Subject);
        Assert.Equal(message, validationResult.Message);
    }
    
    public static void AssertFailed<TException>(this ValidationResult validationResult, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(validationResult);
        Assert.Equal(ValidationStatus.Failed, validationResult.Status);
        Assert.Equal(validationSubject, validationResult.Subject);
        Assert.Equal(message, validationResult.Message);
        var aggregatedException = Assert.IsType<AggregateException>(validationResult.Exception);
        Assert.Contains(aggregatedException.InnerExceptions, x => x.GetType() == typeof(TException));
    }
    
    public static void AssertValid(this ValidationResult validation)
    {
        Assert.Equal(ValidationStatus.Valid, validation.Status);
    }
}