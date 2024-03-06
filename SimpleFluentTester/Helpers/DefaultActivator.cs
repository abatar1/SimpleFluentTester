using System;

namespace SimpleFluentTester.Helpers;

internal sealed class DefaultActivator : IActivator
{
    public object? CreateInstance(Type type) => Activator.CreateInstance(type);
}