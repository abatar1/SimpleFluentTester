using System.Text;
using NUnit.Framework;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.NUnitRunner;

internal static class TestCaseExtensions
{
    internal static void AssertTestCase(this CompletedTestCase<object> completedTestCase)
    {
        if (completedTestCase.ValidationStatus == ValidationStatus.Ignored)
            return;

        if (completedTestCase.ValidationStatus != ValidationStatus.Valid)
        {
            var validationMessageBuilder = new StringBuilder();
            validationMessageBuilder.AppendLine($"Test case {completedTestCase.Number} validation failed");
            completedTestCase.ValidationResults.AssertValidationResults(validationMessageBuilder);
            return;
        }

        var expectedMessage = new NUnitString($"Test case {completedTestCase.Number} has failed assertion");
        Assert.That(completedTestCase.Expected, Is.EqualTo(completedTestCase.Assert?.Output?.Value),
            expectedMessage);
    }
    
    internal static void AssertValidationResults(this IEnumerable<ValidationResult> validationResults, 
        StringBuilder messageBuilder)
    {
        var nonValidValidations = validationResults
            .Where(x => !x.IsValid)
            .ToList();

        foreach (var validationResult in nonValidValidations)
        {
            messageBuilder.AppendLine($"-Subject: {validationResult.Subject}");
            messageBuilder.AppendLine($"Message: {validationResult.Message}");
        }
        
        var validationMessage = new NUnitString(messageBuilder.ToString());
        Assert.That(nonValidValidations.Count == 0, Is.True, validationMessage);
    }
}