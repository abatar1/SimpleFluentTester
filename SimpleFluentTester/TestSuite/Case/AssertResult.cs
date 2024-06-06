using System;
using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.TestSuite.Case;

/// <summary>
/// Represents assertion result of the test case, could be calculated, ignored or executed with an exception.
/// </summary>
public sealed record AssertResult(
    IComparedObject Output,
    AssertStatus Status,
    Exception? Exception = null,
    string? Message = null)
{
    public IComparedObject Output { get; } = Output;
        
    public AssertStatus Status { get; } = Status;
        
    public Exception? Exception { get; } = Exception;
        
    public string? Message { get; } = Message;

    public static AssertResult Ignored => new(new NullObject(), AssertStatus.Ignored);
}