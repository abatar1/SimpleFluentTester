using System.Collections;
using System.Reflection;
using SimpleFluentTester.Core.Reporter;

namespace SimpleFluentTester.Examples;

internal sealed class CustomReporterFactory : BaseTestRunReporterFactory
{
    public override ITestRunReporter GetReporter<TOutput>(IList innerTestResult, MethodInfo methodInfo)
    {
        return new CustomReporter(innerTestResult, methodInfo);
    }
}