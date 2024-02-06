using System.Collections;
using System.Reflection;

namespace SimpleTester.Core.Reporter;

public abstract class BaseTestRunReporterFactory
{
    public abstract ITestRunReporter GetReporter<TOutput>(IList testCases, MethodInfo methodInfo);
}