using System;

namespace SimpleFluentTester.Helpers;

public interface IActivator
{
    object CreateInstance(Type type, params object?[] args);
}