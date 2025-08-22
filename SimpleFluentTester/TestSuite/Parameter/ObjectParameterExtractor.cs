using System;
using System.Reflection;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.TestSuite.Parameter;

public static class ObjectParameterExtractor
{
    public static ParameterInfo ExtractExpectedParameter(ITestClause testClause)
    {
        var parameterExpected = (ParameterObject)testClause.Expected;
        return parameterExpected.Parameter.LazyParameterInfo.Value;
    }
    
    public static Type ExtractExpectedParameterType(ITestClause testClause)
    {
        var info = ExtractExpectedParameter(testClause);
        return info.ParameterType;
    }
    
    public static object? ExtractExpectedParameterValue(ITestClause testClause)
    {
        return (testClause.Expected.Value as IComparedObject)?.Value;
    }
}