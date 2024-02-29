using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.Entities;

namespace SimpleFluentTester.TestRun;

public sealed class TestCaseInputBuilder<TOutput>(TOutput? expected, TestRunBuilderContext<TOutput> context)
{
    /// <summary>
    /// Defines input parameters; their types should match the types of the tested method's input parameters and should be in the same order.
    /// </summary>
    public TestRunBuilder<TOutput> WithInput(params object?[] inputs)
    {
        var calculatedResult = new Lazy<CalculatedTestResult<TOutput>>(() =>
        {
            if (context.Operation.Value == null)
                throw new InvalidOperationException("Value of operation has been tried to be calculated before operation has been set, this is most likely a bug.");
            
            ValidateInputs(context.OperationParameters, inputs);
            return ExecuteTestIteration(context, inputs, expected);
        });
        
        var innerResult = new TestCase<TOutput>(inputs, expected, calculatedResult, false, context.TestCases.Count + 1);
        context.TestCases.Add(innerResult);

        return new TestRunBuilder<TOutput>(context);
    }
    
    private static void ValidateInputs(IReadOnlyCollection<ParameterInfo> operationParameterInfos, object?[] inputs)
    {
        if (inputs.Length != operationParameterInfos.Count)
            throw new TargetParameterCountException($"Invalid inputs number, should be {operationParameterInfos.Count}, but was {inputs.Length}, inputs {string.Join(", ", inputs)}");

        var parametersTypesAreValid = inputs
            .Zip(operationParameterInfos, (input, parameter) => (input, parameter))
            .All(x => ValidateInputType(x.input, x.parameter.ParameterType));
        if (!parametersTypesAreValid)
            throw new InvalidCastException("Passed parameters and expected operation parameters are not equal");
    }

    private static bool ValidateInputType(object? input, Type parameterType)
    {
        var underlyingReturnParameterType = Nullable.GetUnderlyingType(parameterType);
        if (underlyingReturnParameterType == null) 
            return input != null && input.GetType() == parameterType;
        
        if (input == null)
            return true;

        return input.GetType() == underlyingReturnParameterType;
    }
    
    private static CalculatedTestResult<TOutput> ExecuteTestIteration(TestRunBuilderContext<TOutput> context,
        object?[] input, 
        TOutput? expectedOutput)
    {
        var stopwatch = new Stopwatch();
        object? invokeResult;
        try
        {
            stopwatch.Start();
            invokeResult = context.Operation.Value!.Method.Invoke(context.Operation.Value.Target, input);
            stopwatch.Stop();
        }
        catch (TargetInvocationException e)
        {
            return new CalculatedTestResult<TOutput>(false, null, e.InnerException, stopwatch.Elapsed);
        }

        var output = (TOutput?)invokeResult;
        
        bool passed;
        if (context.OutputUnderlyingType != null && expectedOutput == null)
        {
            passed = invokeResult == (object?)expectedOutput;
        }
        else
        {
            try
            {
                passed = context.Comparer?.Invoke(output, expectedOutput) ?? output?.Equals(expectedOutput) ?? false;
            }
            catch (TargetInvocationException e)
            {
                return new CalculatedTestResult<TOutput>(false, null, e.InnerException, stopwatch.Elapsed);
            }
        }
        
        return new CalculatedTestResult<TOutput>(passed, new ValueWrapper<TOutput>(output), null, stopwatch.Elapsed);
    }
}