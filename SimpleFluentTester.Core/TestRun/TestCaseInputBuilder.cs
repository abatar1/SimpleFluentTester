using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.Entities;

namespace SimpleFluentTester.TestRun;

public sealed class TestCaseInputBuilder<TOutput>(TOutput expected, TestRunBuilderContext<TOutput> context)
{
    /// <summary>
    /// Defines input parameters; their types should match the types of the tested method's input parameters and should be in the same order.
    /// </summary>
    public TestRunBuilder<TOutput> WithInput(params object[] inputs)
    {
        var calculatedResult = new Lazy<CalculatedTestResult<TOutput>>(() =>
        {
            if (context.Operation.Value == null)
                throw new InvalidOperationException("Value of operation has been tried to be calculated before operation has been set, this is most likely a bug.");
            
            ValidateInputs(context.OperationParameters, inputs);
            return ExecuteTestIteration(context.Operation.Value, inputs, expected);
        });
        
        var innerResult = new TestCase<TOutput>(inputs, expected, calculatedResult, true, context.TestCases.Count + 1);
        context.TestCases.Add(innerResult);

        return new TestRunBuilder<TOutput>(context);
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