using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Validators.Core;

internal sealed class ValidationInvoker<TOutput>(
    IValidator validator, 
    ITestSuiteBuilderContext<TOutput> context, 
    IValidatedObject validatedObject) : IValidationInvoker
{
    public ValidationResult Invoke() => validator.Validate(context, validatedObject);
}