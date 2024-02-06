using System.Collections;
using System.Reflection;
using SimpleTester.Core.Reporter;

namespace SimpleTester.Examples;

internal sealed class CustomReporterFactory : BaseTestRunReporterFactory
{
    public override ITestRunReporter GetReporter<TOutput>(IList innerTestResult, MethodInfo methodInfo)
    {
        return new CustomReporter(innerTestResult, methodInfo);
    }
}