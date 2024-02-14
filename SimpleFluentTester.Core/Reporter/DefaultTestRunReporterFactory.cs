using System.Collections;
using System.Reflection;

namespace SimpleFluentTester.Reporter;

internal sealed class DefaultTestRunReporterFactory : BaseTestRunReporterFactory
{
    public override ITestRunReporter GetReporter<TOutput>(IEnumerable innerTestResult, MethodInfo methodInfo)
    {
        return new DefaultTestRunReporter<TOutput>(innerTestResult, methodInfo);
    }
}