using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Base class that should be used for defining your own custom reporter.
/// </summary>
public abstract class BaseTestRunReporter<TOutput>(TestRunResult<TOutput> testRunResult) : ITestRunReporter
{
    protected readonly TestRunResult<TOutput> TestRunResult = testRunResult;

    public abstract void Report();

    protected virtual ILoggerFactory BuildLoggerFactory()
    {
        return LoggerFactory.Create(loggerBuilder =>
        {
            loggerBuilder.ClearProviders();
            loggerBuilder.AddSimpleConsole(x =>
            {
                x.TimestampFormat = "HH:mm:ss.fff ";
                x.IncludeScopes = true;
                x.ColorBehavior = LoggerColorBehavior.Enabled;
            });
        });
    }
}