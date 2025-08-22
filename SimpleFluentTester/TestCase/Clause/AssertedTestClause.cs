using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.TestCase.Clause;

/// <summary>
/// Represents an asserted test clause used for validating the result of a test case against the expected value.
/// Contains properties for the expected value, actual result, and the assertion outcome.
/// </summary>
public sealed class AssertedTestClause(IComparedObject expected, IComparedObject result, AssertResult assert) : ITestClause
{
    public IComparedObject Expected { get; } = expected;
    
    public IComparedObject Result { get; } = result;

    public AssertResult Assert { get; } = assert;
}