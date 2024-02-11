using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.TestRun;

public sealed class TestRunBuilderContext<TOutput>(
    IEntryAssemblyProvider entryAssemblyProvider,
    IList<TestCase<TOutput>> testCases,
    BaseTestRunReporterFactory reporterFactory,
    ValueWrapper<Delegate> operation)
{
    public IEntryAssemblyProvider EntryAssemblyProvider { get; } = entryAssemblyProvider;

    public IList<TestCase<TOutput>> TestCases { get; } = testCases;

    public BaseTestRunReporterFactory ReporterFactory { get; set; } = reporterFactory;

    public ValueWrapper<Delegate> Operation { get; } = operation;

    private ParameterInfo[]? _operationParameters;
    public ParameterInfo[] OperationParameters
    {
        get
        {
            if (_operationParameters != null)
                return _operationParameters;
            if (Operation.Value == null)
                throw new InvalidOperationException("_operationParameters where accessed before _operation has been initialized, this should be the bug");
            
            _operationParameters = Operation.Value.Method.GetParameters();
            return _operationParameters;
        }
    }
}