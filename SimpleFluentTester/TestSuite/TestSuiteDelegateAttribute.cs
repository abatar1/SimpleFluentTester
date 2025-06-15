using System;

namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Represents an attribute that is used to designate a method as a delegate
/// for a test suite operation within the SimpleFluentTester framework.
/// </summary>
/// <remarks>
/// This attribute can be applied to methods to indicate that the marked method
/// serves as an entry point or operation for a test suite. The method can either
/// be static or instance-based, but certain constraints apply:
/// - Static methods are supported without additional requirements.
/// - For instance-based methods, the declaring type must have a parameterless constructor.
/// Failure to fulfill these requirements will result in runtime exceptions during
/// the setup or execution of the test suite.
/// </remarks>
/// <example>
/// This attribute is primarily utilized internally by the framework to identify
/// and link the methods within the entry assembly that provide test suite operations.
/// Attempting to define multiple methods with this attribute in an entry assembly
/// will result in an exception.
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TestSuiteDelegateAttribute : Attribute;
