using System.Collections;

namespace SimpleFluentTester.Reporter;

internal sealed class EmptyTestRunReporter<TOutput>(IEnumerable innerTestResult)
    : BaseTestRunReporter<TOutput>(innerTestResult, null)
{
    public override void Report()
    {
    }
}