using System;
using System.Linq;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public sealed class ComparedObjectFactory : IComparedObjectFactory
{
    public IComparedObject Wrap(object? obj)
    {
        if (obj == null)
            return new NullObject();
        
        var objType = obj.GetType();
        var outputUnderlyingType = Nullable.GetUnderlyingType(objType);

        if (outputUnderlyingType != null)
            return new ValueObject(obj, outputUnderlyingType);

        if (obj is Exception exception)
            return new ExceptionObject(exception, objType);

        return new ValueObject(obj, objType);
    }

    public IComparedObject[] WrapMany(object?[] objects)
    {
        return objects.Select(Wrap).ToArray();
    }
}