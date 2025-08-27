using System;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.TestCase;

/// <summary>
/// Represents an assertion result of the test case, could be calculated, ignored or executed with an exception.
/// </summary>
public sealed record AssertResult(
    AssertStatus Status,
    Exception? Exception = null,
    string? Message = null)
{
    public AssertStatus Status { get; } = Status;
        
    public Exception? Exception { get; } = Exception;
        
    public string? Message { get; } = Message;

    public static AssertResult Ignored => new(AssertStatus.Ignored);
}