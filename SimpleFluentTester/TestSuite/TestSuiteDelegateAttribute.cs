using System;

namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Indicates that specifically this method should be tested. Can only be used in conjunction with the method. T
/// There cannot be more than one in the project. Should be used on a static method, or the class defining a non-static method must have an empty constructor.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TestSuiteDelegateAttribute : Attribute;
