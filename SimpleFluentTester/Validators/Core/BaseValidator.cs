using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.Validators.Core;

internal abstract class BaseValidator : IValidator
{
    private readonly string _key;

    protected BaseValidator()
    {
        _key = GetType().Name;
    }

    public virtual string Key => _key;

    public abstract ValidationResult Validate<TOutput>(
        ITestSuiteBuilderContext<TOutput> context,
        IValidatedObject validatedObject);

    public bool Equals(IValidator other)
    {
        return Key == other.Key;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BaseValidator)obj);
    }

    public override int GetHashCode()
    {
        return _key.GetHashCode();
    }
}