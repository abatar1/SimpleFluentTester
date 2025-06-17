using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Helpers.Extensions;

public static class TestValidationExtensions
{
    public static void AssertNonValid(this IValidatedObject validated, ValidationSubject subject, params string[] messages)
    {
        Assert.NotNull(validated);

        Assert.False(validated.Validations.IsValid());
        
        var validations = validated.Validations.GetInvalid().ToList();
        Assert.NotEmpty(validations);
        
        for (var i = 0; i < validations.Count; i++)
        {
            Assert.NotNull(validations[i]);
            Assert.Equal(subject, validations[i].Subject);
            Assert.Equal(messages[i], validations[i].Message);
        }
      
    }

    public static void AssertValid(this IValidatedObject validated)
    {
        Assert.Empty(validated.Validations.GetInvalid());
        Assert.True(validated.Validations.IsValid());
    }
    
    public static void AssertFailed<TException>(this IValidatedObject validated, ValidationSubject validationSubject, string validationMessage, string? innerMessage = null)
        where TException: Exception
    {
        Assert.NotNull(validated);
        
        Assert.False(validated.Validations.IsValid());

        var validations = validated.Validations.GetInvalid();
        Assert.NotEmpty(validations);
        
        var validation = validations
            .FirstOrDefault(x => x.Subject == validationSubject);
        Assert.NotNull(validation);
        
        validation.AssertFailed<TException>(validationSubject, validationMessage, innerMessage);
    }
    
    public static void AssertInvalid(this ValidationResult validationResult, ValidationSubject validationSubject, string message)
    {
        Assert.NotNull(validationResult);
        Assert.Equal(ValidationStatus.NonValid, validationResult.Status);
        Assert.Equal(validationSubject, validationResult.Subject);
        Assert.Equal(message, validationResult.Message);
    }
    
    public static void AssertFailed<TException>(this ValidationResult validationResult, ValidationSubject validationSubject, string validationMessage, string? innerMessage = null)
        where TException: Exception
    {
        Assert.NotNull(validationResult);
        Assert.Equal(ValidationStatus.Failed, validationResult.Status);
        Assert.Equal(validationSubject, validationResult.Subject);
        Assert.Equal(validationMessage, validationResult.Message);
        var exception = Assert.IsType<TException>(validationResult.Exception);
        if (innerMessage != null)
            Assert.Equal(innerMessage, exception.Message);
    }
    
    public static void AssertValid(this ValidationResult validation)
    {
        Assert.Equal(ValidationStatus.Valid, validation.Status);
    }
}