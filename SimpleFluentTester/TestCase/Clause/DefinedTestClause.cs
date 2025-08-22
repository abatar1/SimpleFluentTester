using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.TestCase.Clause;

/// <summary>
/// Represents a clause used in test case definitions, encapsulating the expected object to be tested.
/// </summary>
public sealed class DefinedTestClause(IComparedObject expected) : ITestClause
{
    public IComparedObject Expected { get; } = expected;
}