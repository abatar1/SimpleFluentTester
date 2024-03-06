using Microsoft.Extensions.Logging;

namespace SimpleFluentTester.Reporter;

public sealed record PrintableTestSuiteResult(LogLevel LogLevel, EventId EventId, string Message)
{
    public LogLevel LogLevel { get; } = LogLevel;
    
    public EventId EventId { get; } = EventId;
    
    public string Message { get; } = Message;
}