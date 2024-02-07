using System.Collections;
using System.Reflection;

namespace SimpleFluentTester.Reporter;

public sealed class DefaultTestRunReporterFactory : BaseTestRunReporterFactory
{
    public override ITestRunReporter GetReporter<TOutput>(IList innerTestResult, MethodInfo methodInfo)
    {
        return new DefaultTestRunReporter<TOutput>(innerTestResult, methodInfo);
    }
}