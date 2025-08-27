using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Helpers;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Helpers.Extensions;

internal static class AssertValidationExtensions
{
    public static void AssertValid(this IValidatedObject validated)
    {
        Assert.Empty(validated.GetNonValidValidations());
        Assert.True(validated.IsValid());
    }
    
    public static void AssertValid(this SubjectValidation subjectValidation)
    {
        Assert.Empty(subjectValidation.GetNonValidValidations());
        Assert.True(subjectValidation.IsValid());
    }
    
    public static void AssertNonValid(this IValidatedObject validated, ValidationSubject subject, string message)
    {
        Assert.NotNull(validated);
        Assert.False(validated.IsValid());
        
        var validations = validated.GetNonValidValidations().ToList();
        Assert.NotEmpty(validations);
        
        var checkingValidation = validations.FirstOrDefault(x => x.Subject == subject);
        Assert.NotNull(checkingValidation);
        Assert.Equal(ValidationStatus.NonValid, checkingValidation.Status);
        Assert.Equal(message, checkingValidation.Message);
        Assert.Equal(subject, checkingValidation.Subject);
    }
    
    public static void AssertNonValid(this SubjectValidation subjectValidation, ValidationSubject subject, params string[] messages)
    {
        AssertNonValid(subjectValidation.Validations, subject, messages);
    }
    
    public static void AssertFailedValidation<TException>(this IValidatedObject validated, ValidationSubject validationSubject, string validationMessage, string? innerMessage = null)
        where TException: Exception
    {
        Assert.NotNull(validated);
        
        Assert.False(validated.IsValid());

        var validations = validated.GetNonValidValidations();
        Assert.NotEmpty(validations);
        
        var validation = validations
            .FirstOrDefault(x => x.Subject == validationSubject);
        Assert.NotNull(validation);
        
        validation.AssertFailed<TException>(validationSubject, validationMessage, innerMessage);
    }
    
    private static void AssertFailed<TException>(this ValidationResult validationResult, ValidationSubject validationSubject, string validationMessage, string? innerMessage = null)
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
    
    private static void AssertNonValid(this IList<ValidationResult> validationResults, ValidationSubject subject, params string[] messages)
    {
        Assert.NotEmpty(validationResults);
        
        for (var i = 0; i < validationResults.Count; i++)
        {
            Assert.NotNull(validationResults[i]);
            Assert.Equal(subject, validationResults[i].Subject);
            Assert.Equal(messages[i], validationResults[i].Message);
        }
    }
}