using System;

namespace SimpleFluentTester.TestRun;

internal sealed class DefaultActivator : IActivator
{
    public object? CreateInstance(Type type) => Activator.CreateInstance(type);
}