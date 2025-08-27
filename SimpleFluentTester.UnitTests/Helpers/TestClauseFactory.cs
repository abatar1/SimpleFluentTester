using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;

namespace SimpleFluentTester.UnitTests.Helpers;

internal static class TestClauseFactory
{
    public static List<DefinedTestClause> DefineFromValue(object? value)
    {
        var expected = ComparedObjectFactory.Wrap(value);
        return [new(expected)];
    }
    
    public static List<DefinedTestClause> DefineFromParameter(object? value, DeferredOperationParameter parameter)
    {
        var expected = ComparedObjectFactory.WrapParameter(value, parameter);
        return [new(expected)];
    }
}