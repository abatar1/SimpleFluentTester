using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Extensions;

public static class TestValidationExtensions
{
    private static readonly ValidationUnpacker ValidationUnpacker = new();
    
    public static void AssertNotValidValidation(this IValidated validated, ValidationSubject subject, string message)
    {
        var unpacked = ValidationUnpacker.Unpack(validated);
        unpacked.AssertInvalid(subject, message);
    }
    
    public static void AssertValidValidation(this IValidated context)
    {
        var unpacked = ValidationUnpacker.Unpack(context);
        unpacked.AssertValid();
    }
    
    public static void AssertInvalid(this ValidationUnpacked validationUnpacked, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(validationUnpacked);
        
        Assert.False(validationUnpacked.IsValid);
        var validations = validationUnpacked.GetNonValid();
        Assert.NotEmpty(validations);
        var validation = validations
            .FirstOrDefault(x => x.Subject == validationSubject);
        Assert.NotNull(validation);
        validation.AssertInvalid(validationSubject, message);
    }
    
    public static void AssertFailed<TException>(this ValidationUnpacked validationUnpacked, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(validationUnpacked);
        
        Assert.False(validationUnpacked.IsValid);
        var validations = validationUnpacked.GetNonValid();
        Assert.NotEmpty(validations);
        var validation = validations
            .FirstOrDefault(x => x.Subject == validationSubject);
        Assert.NotNull(validation);
        validation.AssertFailed<TException>(validationSubject, message);
    }
    
    public static void AssertValid(this ValidationUnpacked validationUnpacked)
    {
        Assert.Empty(validationUnpacked.GetNonValid());
        Assert.True(validationUnpacked.IsValid);
    }
    
    public static void AssertInvalid(this ValidationResult validation, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(validation);
        Assert.Equal(ValidationStatus.NonValid, validation.Status);
        Assert.Equal(validationSubject, validation.Subject);
        Assert.Equal(message, validation.Message);
    }
    
    public static void AssertFailed<TException>(this ValidationResult validation, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(validation);
        Assert.Equal(ValidationStatus.Failed, validation.Status);
        Assert.Equal(validationSubject, validation.Subject);
        Assert.Equal(message, validation.Message);
        var aggregatedException = Assert.IsType<AggregateException>(validation.Exception);
        Assert.Contains(aggregatedException.InnerExceptions, x => x.GetType() == typeof(TException));
    }
    
    public static void AssertValid(this ValidationResult validation)
    {
        Assert.Equal(ValidationStatus.Valid, validation.Status);
    }
}