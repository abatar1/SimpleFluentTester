namespace SimpleFluentTester.Helpers;

public sealed record ValueWrapper<TOutput>
{
    public ValueWrapper()
    {
    }

    public ValueWrapper(TOutput? value)
    {
        Value = value;
    }
    
    public TOutput? Value { get; set; }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
}