using System.Collections;
using System.Reflection;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.Examples;

internal sealed class CustomReporter(IList innerTestResults, MethodInfo methodInfo) : BaseTestRunReporter<int>(innerTestResults, methodInfo)
{
    public override void Report()
    {
        Console.WriteLine($"\nCustom console test reporter for method [{MethodInfo}]\n");
        foreach (var innerTestResult in InnerTestResults)
        {
            Console.WriteLine(innerTestResult);
            Console.WriteLine();
        }
    }
}