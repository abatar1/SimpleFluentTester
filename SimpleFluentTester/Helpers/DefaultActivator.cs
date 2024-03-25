using System;

namespace SimpleFluentTester.Helpers;

internal sealed class DefaultActivator : IActivator
{
    public object CreateInstance(Type type, params object?[] args) => Activator.CreateInstance(type, args);
}