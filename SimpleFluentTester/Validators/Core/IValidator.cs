using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.Validators.Core;

public interface IValidator
{
    string Key { get; }
    
    ValidationResult Validate<TOutput>(ITestSuiteBuilderContext<TOutput> context, IValidatedObject validatedObject);
}