using System;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Validators.Core;

public interface IValidator : IEquatable<IValidator>
{
    string Key { get; }
    
    ValidationResult Validate<TOutput>(ITestSuiteBuilderContext<TOutput> context, IValidatedObject validatedObject);
}