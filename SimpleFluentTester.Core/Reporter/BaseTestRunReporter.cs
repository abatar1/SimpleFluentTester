using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using SimpleFluentTester.Core.Entities;

namespace SimpleFluentTester.Core.Reporter;

public abstract class BaseTestRunReporter<TOutput>(IList innerTestResults, MethodInfo methodInfo) : ITestRunReporter
{
    protected readonly IList<TestCase<TOutput>> InnerTestResults = (IList<TestCase<TOutput>>) innerTestResults;

    protected readonly MethodInfo MethodInfo = methodInfo;

    public abstract void Report();

    protected virtual ILoggerFactory BuildLoggerFactory()
    {
        return LoggerFactory.Create(loggerBuilder => loggerBuilder.AddConsole());
    }
}