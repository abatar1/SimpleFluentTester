using System.Text;
using NUnit.Framework;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.NUnitRunner.InternalTests;

[TestFixture]
internal sealed class ContextValidationTests
{
    public static TestSuiteResult<object>.ContextValidationResult? ValidationResult;

    [Test]
    public void TestMethod()
    {
        if (ValidationResult == null)
        {
            Assert.Fail($"{nameof(ValidationResult)} is not set, possibly a bug.");
            return;
        }
        
        if (ValidationResult.Status == ValidationStatus.Ignored)
            return;
        
        var validationMessageBuilder = new StringBuilder();
        validationMessageBuilder.AppendLine("Test case context validation failed.");
        ValidationResult.Results.AssertValidationResults(validationMessageBuilder);
    }
}