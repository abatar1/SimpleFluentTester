using System;
using System.Reflection;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.TestSuite.Parameter;

public static class ObjectParameterExtractor
{
    public static ParameterInfo ExtractExpectedParameter(ITestCase testCase)
    {
        var parameterExpected = (ParameterObject)testCase.Expected;
        return parameterExpected.Parameter.LazyParameterInfo.Value;
    }
    
    public static Type ExtractExpectedParameterType(ITestCase testCase)
    {
        var info = ExtractExpectedParameter(testCase);
        return info.ParameterType;
    }
}