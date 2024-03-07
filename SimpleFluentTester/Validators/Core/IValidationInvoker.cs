using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.Validators.Core;

public interface IValidationInvoker
{
    ValidationResult Invoke<TOutput>(ITestSuiteBuilderContext<TOutput> context);
}