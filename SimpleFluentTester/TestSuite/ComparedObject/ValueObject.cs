using System;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public sealed class ValueObject(object value, Type type) : IComparedObject
{
    public ComparedObjectVariety Variety => ComparedObjectVariety.Value;

    public Type Type { get; } = type;

    public object Value => value;
    
    public override string ToString()
    {
        return Value.ToString();
    }
}