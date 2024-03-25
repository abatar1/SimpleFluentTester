using System;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.Validators.Core;

public static class ValidatedTypes
{
    public static Type Context { get; } = typeof(TestSuiteContext);

    public static Type TestCase { get; } = typeof(TestCase);
}