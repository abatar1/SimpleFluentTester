using Microsoft.Extensions.Logging;

namespace SimpleFluentTester.Reporter.Console;

public sealed record ConsoleTestSuiteResult(LogLevel LogLevel, EventId EventId, string Message)
{
    public LogLevel LogLevel { get; } = LogLevel;
    
    public EventId EventId { get; } = EventId;
    
    public string Message { get; } = Message;
}