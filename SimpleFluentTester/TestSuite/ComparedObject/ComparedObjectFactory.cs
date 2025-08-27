using System;
using System.Linq;
using SimpleFluentTester.TestSuite.Parameter;

namespace SimpleFluentTester.TestSuite.ComparedObject;

internal static class ComparedObjectFactory
{
    /// <summary>
    /// Wraps the given object into an implementation of <see cref="IComparedObject"/>.
    /// Depending on the object's type, it creates a corresponding <see cref="IComparedObject"/> that encapsulates
    /// the value and its associated metadata.
    /// </summary>
    /// <param name="obj">The object to wrap. Can be null.</param>
    /// <returns>An instance of <see cref="IComparedObject"/> that represents the encapsulated object.</returns>
    public static IComparedObject Wrap<T>(T? obj)
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

    /// <summary>
    /// Wraps an object and a <see cref="DeferredOperationParameter"/> into a <see cref="ParameterObject"/>.
    /// This allows handling parameter-specific metadata along with the object itself.
    /// </summary>
    /// <param name="obj">The object to wrap.</param>
    /// <param name="parameter">The deferred operation parameter associated with the object.</param>
    /// <returns>A <see cref="ParameterObject"/> that encapsulates the object and its parameter.</returns>
    public static IComparedObject WrapParameter<T>(T? obj, DeferredOperationParameter parameter)
    {
        return new ParameterObject(Wrap(obj), parameter);
    }

    /// <summary>
    /// Wraps an array of objects into an array of <see cref="IComparedObject"/> instances.
    /// Each object in the input array is individually wrapped into a corresponding <see cref="IComparedObject"/>
    /// that encapsulates its value and associated metadata.
    /// </summary>
    /// <param name="objects">An array of objects to wrap. Each object can be null.</param>
    /// <returns>An array of <see cref="IComparedObject"/> instances representing the encapsulated objects.</returns>
    public static IComparedObject[] WrapMany(object?[] objects)
    {
        return objects.Select(Wrap).ToArray();
    }

    /// <summary>
    /// Returns a new instance of <see cref="NullObject"/> that represents a null value.
    /// This encapsulates the concept of a null object within the <see cref="IComparedObject"/> system.
    /// </summary>
    /// <returns>An instance of <see cref="NullObject"/> representing a null value.</returns>
    public static IComparedObject Null() => new NullObject();
}