using System.Collections.Generic;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite.Parameter;

internal sealed class ParameterExpectationBuilder(
    ITestSuiteContextContainer container, 
    DeferredOperationParameter operationParameter, 
    List<DefinedTestClause> testClauses
    ) : IParameterExpectationBuilder
{
    public ITestCaseBuilder ToBe<T>(T? expected)
    {
        var comparedObj = ComparedObjectFactory.WrapParameter(expected, operationParameter);
        
        var testClause = new DefinedTestClause(comparedObj);
        testClauses.Add(testClause);
        
        return new TestCaseBuilder(container, testClauses);
    }

    public ITestCaseBuilder ToBe<T>(IEnumerable<T?> expected)
    {
        var comparedObj = ComparedObjectFactory.WrapParameter(expected, operationParameter);
        
        var testClause = new DefinedTestClause(comparedObj);
        testClauses.Add(testClause);
        
        return new TestCaseBuilder(container, testClauses);
    }

    public ITestCaseBuilder ToBe<T>(T[] expected)
    {
        var comparedObj = ComparedObjectFactory.WrapParameter(expected, operationParameter);
        
        var testClause = new DefinedTestClause(comparedObj);
        testClauses.Add(testClause);
        
        return new TestCaseBuilder(container, testClauses);
    }
}