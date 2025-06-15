using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite.Parameter;

internal sealed class ParameterExpectationBuilder(ITestSuiteContextContainer container, DeferredOperationParameter operationParameter) : IParameterExpectationBuilder
{
    public ITestCaseBuilder ToBe<T>(T expected)
    {
        var comparedObj = ComparedObjectFactory.WrapParameter(expected, operationParameter);
        return new TestCaseBuilder(container, comparedObj);
    }
}