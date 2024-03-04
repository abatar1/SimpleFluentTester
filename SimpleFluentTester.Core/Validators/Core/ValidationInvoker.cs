using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Validators.Core;

public sealed record ValidationInvoker<TOutput>(
    IValidator Validator, 
    TestSuiteBuilderContext<TOutput> Context, 
    IValidatedObject ValidatedObject)
{
    public ValidationResult Invoke() => Validator.Validate(Context, ValidatedObject);
    
    public IValidator Validator { get; } = Validator;
    
    public TestSuiteBuilderContext<TOutput> Context { get; } = Context;
    
    public IValidatedObject ValidatedObject { get; } = ValidatedObject;
}