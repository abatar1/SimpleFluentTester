using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite.Parameter;

internal static class ExpectParameterFactory
{
    private static readonly ConcurrentDictionary<string, Lazy<OperationParametersInfo>> ParametersInfoMap = new();

    public static DeferredOperationParameter Create(ITestSuiteContextContainer container, string parameterName)
    {
        if (container.Context.Operation?.Method.Name == null)
            throw new InvalidOperationException("Operation delegate accessed before initialization, seems like a bug");
        var lazyParameterInfo = ParametersInfoMap.GetOrAdd(container.Context.Operation.Method.Name, 
            _ => new Lazy<OperationParametersInfo>(() => CreateOperationParametersInfo(container.Context.Operation)));
        return new DeferredOperationParameter(new Lazy<ParameterInfo>(() => lazyParameterInfo.Value.ParametersByName[parameterName]));
    }
    
    public static DeferredOperationParameter Create(ITestSuiteContextContainer container, int parameterPosition)
    {
        if (container.Context.Operation?.Method.Name == null)
            throw new InvalidOperationException("Operation delegate accessed before initialization, seems like a bug");
        var lazyParameterInfo = ParametersInfoMap.GetOrAdd(container.Context.Operation.Method.Name, 
            _ => new Lazy<OperationParametersInfo>(() => CreateOperationParametersInfo(container.Context.Operation)));
        return new DeferredOperationParameter(new Lazy<ParameterInfo>(() => lazyParameterInfo.Value.ParametersByPosition[parameterPosition]));
    }

    private static OperationParametersInfo CreateOperationParametersInfo(Delegate? operation)
    {
        if (operation == null)
            throw new InvalidOperationException("Operation delegate accessed before initialization, seems like a bug");
        
        var methodInfo = operation.Method;
        var parameters = methodInfo.GetParameters();
        
        Dictionary<string, ParameterInfo> parametersByName = new Dictionary<string, ParameterInfo>();
        if (parameters.Select(x => x.Name).Distinct().Count() == parameters.Length)
            parametersByName = parameters.ToDictionary(x => x.Name, x => x);

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