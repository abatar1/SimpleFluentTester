namespace SimpleFluentTester.UnitTests.TestObjects;

public class EquatableTestObject(int value) : IEquatable<EquatableTestObject>
{
    public int Value { get; } = value;

    public bool Equals(EquatableTestObject? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EquatableTestObject)obj);
    }

    public override int GetHashCode()
    {
        return Value;
    }
}