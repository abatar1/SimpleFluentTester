using System;
using SimpleTester.Core.Reporter;

namespace SimpleTester.Core.TestRun;

public sealed class TestRunOperationBuilder
{
    private readonly BaseTestRunReporterFactory _reporterFactory;

    internal TestRunOperationBuilder(BaseTestRunReporterFactory reporterFactory)
    {
        _reporterFactory = reporterFactory;
    }
    
    public TestRunBuilder<TOutput> UseOperation<TOutput>(Delegate operation)
    {
        _ = operation ?? throw new ArgumentNullException(nameof(operation));

        if (operation.Method.ReturnParameter?.ParameterType != typeof(TOutput))
            throw new InvalidCastException($"{nameof(UseOperation)} thrown an exception, operation return type is not the same as used generic type.");

        return new TestRunBuilder<TOutput>(operation, _reporterFactory);
    }
}