using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Validators.Core;

internal sealed class ValidationInvoker<TOutput>(
    IValidator validator, 
    ITestSuiteBuilderContext<TOutput> context, 
    IValidatedObject validatedObject) : IValidationInvoker
{
    public ValidationResult Invoke() => Validator.Validate(Context, ValidatedObject);
    
    public IValidator Validator { get; } = validator;
    
    public ITestSuiteBuilderContext<TOutput> Context { get; } = context;
    
    public IValidatedObject ValidatedObject { get; } = validatedObject;
}