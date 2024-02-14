using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.TestRun;

public sealed class TestRunBuilderContext<TOutput>(
    IEntryAssemblyProvider entryAssemblyProvider,
    IActivator activator,
    IList<TestCase<TOutput>> testCases,
    BaseTestRunReporterFactory reporterFactory,
    ValueWrapper<Delegate> operation,
    Func<TOutput, TOutput, bool>? comparer,
    bool shouldBeExecuted)
{
    public bool ShouldBeExecuted { get; } = shouldBeExecuted;
    
    public IEntryAssemblyProvider EntryAssemblyProvider { get; } = entryAssemblyProvider;

    public IActivator Activator { get; } = activator;

    public IList<TestCase<TOutput>> TestCases { get; } = testCases;

    public BaseTestRunReporterFactory ReporterFactory { get; set; } = reporterFactory;

    public ValueWrapper<Delegate> Operation { get; } = operation;
    
    public Func<TOutput, TOutput, bool>? Comparer { get; } = comparer;

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