using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.TestCase.Clause;

/// <summary>
/// Defines the structure for test clauses, which represent specific parts of a test case,
/// such as the expected values or objects being tested.
/// </summary>
public interface ITestClause
{
    IComparedObject Expected { get; }
}