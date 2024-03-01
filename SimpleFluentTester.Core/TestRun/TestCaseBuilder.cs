using System;
using System.Diagnostics;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestRun;

public sealed class TestCaseBuilder<TOutput>(TOutput? expected, TestRunBuilderContext<TOutput> context)
{
    /// <summary>
    /// Defines input parameters; their types should match the types of the tested method's input parameters and should be in the same order.
    /// </summary>
    public TestRunBuilder<TOutput> WithInput(params object?[] inputs)
    {
        var assert = new Lazy<Assert<TOutput>>(() => ExecuteTestIteration(context, inputs, expected));
        
        var testCase = new TestCase<TOutput>(inputs, expected, assert, context.TestCases.Count + 1);
        
        if (context.IsObjectOutput)
            testCase.RegisterValidator(context, typeof(OperationValidator), new OperationValidatedObject(expected?.GetType()));
        testCase.RegisterValidator(context, typeof(InputsValidator), new InputsValidatedObject(inputs));
            
        context.TestCases.Add(testCase);

        return new TestRunBuilder<TOutput>(context);
    }
    
    private static Assert<TOutput> ExecuteTestIteration(TestRunBuilderContext<TOutput> context,
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
            return new Assert<TOutput>(false, null, e.InnerException, stopwatch.Elapsed);
        }

        TOutput? output;
        try
        {
            output = (TOutput?)invokeResult;
        }
        catch (Exception e)
        {
            return new Assert<TOutput>(false, null, e, stopwatch.Elapsed);
        }

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
                return new Assert<TOutput>(false, null, e.InnerException, stopwatch.Elapsed);
            }
        }
        
        return new Assert<TOutput>(passed, new ValueWrapper<TOutput>(output), null, stopwatch.Elapsed);
    }
}