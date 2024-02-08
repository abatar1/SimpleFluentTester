using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.TestRun;

public sealed class TestCaseInputBuilder<TOutput>
{
    private readonly TOutput _expected;
    private readonly IList<TestCase<TOutput>> _testCases;
    private readonly BaseTestRunReporterFactory _reporterFactory;
    private readonly Delegate _operation;
    private readonly ParameterInfo[] _operationParameters;

    public TestCaseInputBuilder(TOutput expected, 
        IList<TestCase<TOutput>> testCases, 
        BaseTestRunReporterFactory reporterFactory, 
        Delegate operation, 
        ParameterInfo[] operationParameters)
    {
        _expected = expected;
        _testCases = testCases;
        _reporterFactory = reporterFactory;
        _operation = operation;
        _operationParameters = operationParameters;
    }

    public TestRunBuilder<TOutput> WithInput(params object[] inputs)
    {
        ValidateInputs(_operationParameters, inputs);
        
        var calculatedResult = new Lazy<CalculatedTestResult<TOutput>>(() => ExecuteTestIteration(_operation, inputs, _expected));
        
        var innerResult = new TestCase<TOutput>(inputs, _expected, calculatedResult, true, _testCases.Count + 1);
        _testCases.Add(innerResult);

        return new TestRunBuilder<TOutput>(_operation, _operationParameters, _reporterFactory, _testCases);
    }
    
    private static void ValidateInputs(IReadOnlyCollection<ParameterInfo> operationParameterInfos, object[] inputs)
    {
        if (inputs.Length != operationParameterInfos.Count)
            throw new TargetParameterCountException($"Invalid inputs number, should be {operationParameterInfos.Count}, but was {inputs.Length}, inputs {string.Join(", ", inputs)}");

        var parametersTypesAreValid = inputs
            .Zip(operationParameterInfos, (input, parameter) => (input, parameter))
            .All(x => x.input.GetType() == x.parameter.ParameterType);
        if (!parametersTypesAreValid)
            throw new InvalidCastException("Passed parameters and method parameters are not equal");
    }
    
    private static CalculatedTestResult<TOutput> ExecuteTestIteration(Delegate operation, object[] input, TOutput expectedOutput)
    {
        var stopwatch = new Stopwatch();
        object? invokeResult;
        try
        {
            stopwatch.Start();
            invokeResult = operation.Method.Invoke(operation.Target, input);
            stopwatch.Stop();
        }
        catch (TargetInvocationException e)
        {
            return new CalculatedTestResult<TOutput>(false, null, e.InnerException, stopwatch.Elapsed);
        }

        if (invokeResult is not TOutput output)
            throw new InvalidCastException($"Couldn't convert invoked test result to {typeof(TOutput)} type");

        return new CalculatedTestResult<TOutput>(Equals(output, expectedOutput), new ValueWrapper<TOutput>(output), null, stopwatch.Elapsed);
    }
}