using System;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Validators.Core;

public interface IValidator : IEquatable<IValidator>
{
    string Key { get; }
    
    ValidationResult Validate<TOutput>(TestRunBuilderContext<TOutput> context, IValidatedObject validatedObject);
}