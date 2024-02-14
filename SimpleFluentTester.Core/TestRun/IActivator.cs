using System;

namespace SimpleFluentTester.TestRun;

public interface IActivator
{
    object? CreateInstance(Type type);
}