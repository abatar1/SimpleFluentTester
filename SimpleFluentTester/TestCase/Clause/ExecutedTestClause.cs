using System;
using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.TestCase.Clause;

/// <summary>
/// Represents a clause within a test case that has been executed, encapsulating the expected value,
/// the actual result, and the comparer used to evaluate the test.
/// </summary>
public sealed class ExecutedTestClause(Delegate comparer, IComparedObject expected, IComparedObject result) : ITestClause
{
    public IComparedObject Expected { get; } = expected;

    public IComparedObject Result { get; } = result;
    
    public Delegate Comparer { get; } = comparer;
}