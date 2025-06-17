namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Represents a delegate used for custom comparison of two values of the same type.
/// </summary>
/// <typeparam name="TExpected">The type of values being compared by the delegate.</typeparam>
/// <param name="a">The first value to compare.</param>
/// <param name="b">The second value to compare.</param>
/// <returns>A boolean value indicating whether the comparison condition was met.</returns>
public delegate bool ComparerDelegate<in TExpected>(TExpected? a, TExpected? b);