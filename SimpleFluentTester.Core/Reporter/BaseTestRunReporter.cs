using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SimpleFluentTester.Entities;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Base class that should be used for defining your own custom reporter.
/// </summary>
public abstract class BaseTestRunReporter<TOutput>(IList innerTestResults, MethodInfo methodInfo) : ITestRunReporter
{
    protected readonly IList<TestCase<TOutput>> InnerTestResults = (IList<TestCase<TOutput>>) innerTestResults;

    protected readonly MethodInfo MethodInfo = methodInfo;

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