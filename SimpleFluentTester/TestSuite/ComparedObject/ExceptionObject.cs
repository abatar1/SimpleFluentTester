using System;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public sealed class ExceptionObject(Exception exception, Type type) : IComparedObject
{
    public ComparedObjectVariety Variety => ComparedObjectVariety.Exception;

    public Type Type { get; } = type;

    public object Value => exception;
    
    public override string ToString()
    {
        return $"Exception {Type}";
    }
}