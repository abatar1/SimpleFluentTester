using System;
using System.Linq;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public static class ComparedObjectFactory
{
    /// <summary>
    /// Wrap up any value into the inner-library <see cref="IComparedObject"/> type. It allows to avoid unexpected null values and handle exceptions as a input values.
    /// </summary>
    public static IComparedObject Wrap(object? obj)
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

    public static IComparedObject[] WrapMany(object?[] objects)
    {
        return objects.Select(Wrap).ToArray();
    }
}