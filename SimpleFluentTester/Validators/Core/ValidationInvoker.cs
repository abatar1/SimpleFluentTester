using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.Validators.Core;

internal sealed class ValidationInvoker(
    IValidator validator, 
    IValidatedObject validatedObject) : IValidationInvoker
{
    public ValidationResult Invoke<TOutput>(ITestSuiteBuilderContext<TOutput> context) => validator.Validate(context, validatedObject);
}