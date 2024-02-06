using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SimpleTester.Core.Entities;

public sealed record TestCase<TOutput>(object[] Inputs, TOutput Expected, Lazy<CalculatedTestResult<TOutput>> LazyResult, bool ShouldBeCalculated, int Iteration)
{
    public Lazy<CalculatedTestResult<TOutput>> LazyResult { get; } = LazyResult;
    
    public object[] Inputs { get; } = Inputs;
    
    public TOutput Expected { get; } = Expected;
    
    public bool ShouldBeCalculated { get; set; } = ShouldBeCalculated;
    
    public int Iteration { get; } = Iteration;
    
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        if (!LazyResult.IsValueCreated)
        {
            stringBuilder.AppendLine($"Test iteration [{Iteration}] not calculated");
            AddInputString(stringBuilder);
            AddExpectedString(stringBuilder);
            return stringBuilder.ToString();
        }

        var calculatedResult = LazyResult.Value;

        var noError = calculatedResult.Passed && calculatedResult.Exception == null;

        stringBuilder.AppendLine($"Test iteration [{Iteration}] {(!noError ? "not " : "")}passed");

        AddInputString(stringBuilder);
        AddExpectedString(stringBuilder);

        if (calculatedResult.Output != null)
            stringBuilder.AppendLine($"Output: '{calculatedResult.Output.Value}'");
      
        stringBuilder.AppendLine($"Elapsed: {calculatedResult.ElapsedTime.TotalMilliseconds:F5}ms");

        if (calculatedResult.Exception != null)
        {
            var exception = calculatedResult.Exception;
            if (exception is TargetInvocationException targetInvocationException)
                exception = targetInvocationException.InnerException;
            stringBuilder.AppendLine($"Exception: {exception}");
        }

        return stringBuilder.ToString();
    }

    private StringBuilder AddInputString(StringBuilder stringBuilder)
    {
        if (Inputs.Length == 1)
            stringBuilder.AppendLine($"Input: '{Inputs}'");
        else
            stringBuilder.AppendLine($"Inputs: {string.Join(", ", Inputs.Select(x => $"'{x}'"))}");
        return stringBuilder;
    }
    
    private StringBuilder AddExpectedString(StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine($"Expected: '{Expected}'");
        return stringBuilder;
    } 
}