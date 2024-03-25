using System;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public sealed class NullObject : IComparedObject
{
    public ComparedObjectVariety Variety => ComparedObjectVariety.Null;

    public Type? Type => null;

    public object? Value => null;

    public override string ToString()
    {
        return "null";
    }
}