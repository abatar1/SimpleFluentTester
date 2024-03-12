using System;
using SimpleFluentTester.Helpers;

namespace SimpleFluentTester.TestCase;

public sealed record Assert<TOutput>(bool Passed, ValueWrapper<TOutput>? Output, Exception? Exception, TimeSpan ElapsedTime)
{
    public bool Passed { get; } = Passed;
    
    public ValueWrapper<TOutput>? Output { get; } = Output;
    
    public Exception? Exception { get; } = Exception;
    
    public TimeSpan ElapsedTime { get; } = ElapsedTime;

    public AssertStatus Status
    {
        get
        {
            if (Passed is false && Exception != null)
                return AssertStatus.NotPassedWithException;
            return Passed ? AssertStatus.Passed : AssertStatus.NotPassed;
        }
    }
}