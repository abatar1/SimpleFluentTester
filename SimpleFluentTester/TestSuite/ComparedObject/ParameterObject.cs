using System;
using SimpleFluentTester.TestSuite.Parameter;

namespace SimpleFluentTester.TestSuite.ComparedObject;

internal sealed class ParameterObject(IComparedObject obj, DeferredOperationParameter parameter) : IComparedObject
{
    public ComparedObjectVariety Variety => ComparedObjectVariety.Parameter;

    public Type? Type => null;

    public DeferredOperationParameter Parameter => parameter;

    public object Value => obj;
    
    public override string ToString()
    {
        return Value.ToString();
    }
}