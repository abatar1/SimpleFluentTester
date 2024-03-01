using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Validators.Core;

public sealed record ValidationInvoker<TOutput>(
    IValidator Validator, 
    TestRunBuilderContext<TOutput> Context, 
    IValidatedObject ValidatedObject)
{
    public ValidationResult Invoke() => Validator.Validate(Context, ValidatedObject);
    
    public IValidator Validator { get; } = Validator;
    
    public TestRunBuilderContext<TOutput> Context { get; } = Context;
    
    public IValidatedObject ValidatedObject { get; } = ValidatedObject;
}