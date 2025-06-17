using System;
using System.Collections;
using System.Collections.Concurrent;

namespace SimpleFluentTester.TestSuite.ComparedObject;

internal static class ValueObjectHelper
{
    private static readonly ConcurrentDictionary<Type, bool> IsArrayMap = new();

    public static bool IsArray(Type type)
    {
        return IsArrayMap.GetOrAdd(type, x => typeof(IEnumerable).IsAssignableFrom(x) && x != typeof(string));
    }
}