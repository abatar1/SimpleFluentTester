using System;
using System.Reflection;

namespace SimpleFluentTester.TestSuite.Parameter;

internal sealed record DeferredOperationParameter(Lazy<ParameterInfo> LazyParameterInfo)
{
    public Lazy<ParameterInfo> LazyParameterInfo { get; } = LazyParameterInfo;
}