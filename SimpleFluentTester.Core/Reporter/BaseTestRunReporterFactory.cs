using System.Collections;
using System.Reflection;

namespace SimpleFluentTester.Core.Reporter;

public abstract class BaseTestRunReporterFactory
{
    public abstract ITestRunReporter GetReporter<TOutput>(IList testCases, MethodInfo methodInfo);
}