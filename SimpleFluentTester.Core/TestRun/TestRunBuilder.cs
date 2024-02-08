using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.TestRun;

public sealed class TestRunBuilder<TOutput>
{
    private readonly List<TestCase<TOutput>> _testCases = [];
    private readonly BaseTestRunReporterFactory _reporterFactory;
    private readonly Delegate _operation;
    private readonly ParameterInfo[] _operationParameters;
    private int _testIteration;

    internal TestRunBuilder(Delegate operation, BaseTestRunReporterFactory reporterFactory)
    {
        _reporterFactory = reporterFactory ?? throw new ArgumentNullException(nameof(reporterFactory));;
        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        _operationParameters = _operation.Method.GetParameters();
    }

    public TestRunBuilder<TOutput> AddTestCase(TOutput expected, params object[] inputs)
    {
        if (inputs.Length != _operationParameters.Length)
            throw new TargetParameterCountException($"Invalid inputs number, should be {_operationParameters.Length}, but was {inputs.Length}, inputs {string.Join(", ", inputs)}");

        var parametersTypesAreValid = inputs
            .Zip(_operationParameters, (input, parameter) => (input, parameter))
            .All(x => x.input.GetType() == x.parameter.ParameterType);
        if (!parametersTypesAreValid)
            throw new InvalidCastException("Passed parameters and method parameters are not equal");
        
        var calculatedResult = new Lazy<CalculatedTestResult<TOutput>>(() => ExecuteTestIteration(inputs, expected));
        _testIteration += 1;
        var innerResult = new TestCase<TOutput>(inputs, expected, calculatedResult, true, _testIteration);
        _testCases.Add(innerResult);
        
        return this;
    }
    
    public BaseTestRunReporter<TOutput> Run(params int[] testIterations)
    {
        var testIterationsHash = new HashSet<int>(testIterations);
        
        if (testIterationsHash.Count != 0 && (testIterationsHash.Count > _testCases.Count || testIterationsHash.Max() > _testCases.Count))
            throw new InvalidOperationException("Invalid test case iteration was given as input");

        var executedTestCases = _testCases
            .Select((testResult, it) => (testResult, it + 1))
            .Select(x =>
            {
                var shouldBeCalculated = testIterationsHash.Count == 0 || (testIterationsHash.Count != 0 && testIterationsHash.Contains(x.Item2));
                if (shouldBeCalculated)
                    _ = x.testResult.LazyResult.Value;
                return x.testResult with { ShouldBeCalculated = shouldBeCalculated };
            })
            .ToList();

        return (BaseTestRunReporter<TOutput>)_reporterFactory.GetReporter<TOutput>(executedTestCases, _operation.Method);
    }
    
    private CalculatedTestResult<TOutput> ExecuteTestIteration(object[] input, TOutput expected)
    {
        var stopwatch = new Stopwatch();
        object? invokeResult;
        try
        {
            stopwatch.Start();
            invokeResult = _operation.Method.Invoke(_operation.Target, input);
            stopwatch.Stop();
        }
        catch (TargetInvocationException e)
        {
            return new CalculatedTestResult<TOutput>(false, null, e.InnerException, stopwatch.Elapsed);
        }

        if (invokeResult is not TOutput output)
            throw new InvalidCastException($"Couldn't convert invoked test result to {typeof(TOutput)} type");

        return new CalculatedTestResult<TOutput>(Equals(output, expected), new ValueWrapper<TOutput>(output), null, stopwatch.Elapsed);
    }
}