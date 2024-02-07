using System;

namespace SimpleFluentTester.Entities;

public sealed record CalculatedTestResult<TOutput>(bool Passed, ValueWrapper<TOutput>? Output, Exception? Exception, TimeSpan ElapsedTime)
{
    public bool Passed { get; } = Passed;
    
    public ValueWrapper<TOutput>? Output { get; } = Output;
    
    public Exception? Exception { get; } = Exception;
    
    public TimeSpan ElapsedTime { get; } = ElapsedTime;
}