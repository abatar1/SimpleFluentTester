using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.UnitTests.Helpers;

public static class TestClauseFactory
{
    public static IList<DefinedTestClause> DefineFromValue(object? value)
    {
        var expected = ComparedObjectFactory.Wrap(value);
        return new List<DefinedTestClause> { new(expected) };
    }
}