namespace SimpleFluentTester.Entities;

public sealed record ValueWrapper<TOutput>(TOutput Value)
{
    public TOutput Value { get; } = Value;

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
}