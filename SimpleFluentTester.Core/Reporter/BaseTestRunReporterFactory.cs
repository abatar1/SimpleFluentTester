using System.Collections;
using System.Reflection;

namespace SimpleFluentTester.Reporter;

/// <summary>
/// Factory that needs to be inherited to return your custom reporter.
/// </summary>
public abstract class BaseTestRunReporterFactory
{
    public abstract ITestRunReporter GetReporter<TOutput>(IList testCases, MethodInfo methodInfo);
}