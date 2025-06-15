using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite.Parameter;

internal static class ExpectParameterFactory
{
    private static Lazy<OperationParametersInfo> _parametersInfo = new();
    private static bool Initialized { get; set; }

    public static DeferredOperationParameter Create(ITestSuiteContextContainer container, string parameterName)
    {
        TryInitParameters(container);
        return new DeferredOperationParameter(new Lazy<ParameterInfo>(() => _parametersInfo.Value.ParametersByName[parameterName]));
    }
    
    public static DeferredOperationParameter Create(ITestSuiteContextContainer container, int parameterPosition)
    {
        TryInitParameters(container);
        return new DeferredOperationParameter(new Lazy<ParameterInfo>(() => _parametersInfo.Value.ParametersByPosition[parameterPosition]));
    }

    private static void TryInitParameters(ITestSuiteContextContainer container)
    {
        if (Initialized)
            return;

        _parametersInfo = new Lazy<OperationParametersInfo>(() => CreateOperationParametersInfo(container.Context.Operation));

        Initialized = true;
    }

    private static OperationParametersInfo CreateOperationParametersInfo(Delegate? operation)
    {
        if (operation == null)
            throw new InvalidOperationException("Operation delegate accessed before initialization, seems like a bug");
        
        var methodInfo = operation.Method;
        var parameters = methodInfo.GetParameters();
        
        var parametersByName = parameters.ToDictionary(x => x.Name, x => x);
        var parametersByPosition = parameters.ToDictionary(x => x.Position, x => x);
        
        return new OperationParametersInfo(parametersByName, parametersByPosition);
    }

    internal sealed record OperationParametersInfo(
        IDictionary<string, ParameterInfo> ParametersByName,
        IDictionary<int, ParameterInfo> ParametersByPosition)
    {
        public IDictionary<string, ParameterInfo> ParametersByName { get; } = ParametersByName;
        public IDictionary<int, ParameterInfo> ParametersByPosition { get; } = ParametersByPosition;
    }
}