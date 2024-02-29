using System.Collections;
using System.Reflection;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.Examples;

internal sealed class CustomReporterFactory : BaseTestRunReporterFactory
{
    public override ITestRunReporter GetReporter<TOutput>(IEnumerable testCases, MethodInfo methodInfo)
    {
        return new CustomReporter(testCases, methodInfo);
    }
}