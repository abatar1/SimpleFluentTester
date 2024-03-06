using System;
using SimpleFluentTester.Suite;

namespace SimpleFluentTester.Validators.Core
{
    public interface IValidator : IEquatable<IValidator>
    {
        string Key { get; }
    
        ValidationResult Validate<TOutput>(TestSuiteBuilderContext<TOutput> context, IValidatedObject validatedObject);
    }
}