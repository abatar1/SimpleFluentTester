using System;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite;

public interface ITestSuiteBuilder<TOutput>
{
    ITestCaseBuilder<TOutput> Expect(TOutput? expected);

    ITestSuiteBuilder<TOutput> UseOperation(Delegate operation);

    ITestSuiteBuilder<TOutput> WithDisplayName(string displayName);

    ITestSuiteBuilder<TNewOutput> WithComparer<TNewOutput>(Func<TNewOutput?, TNewOutput?, bool> comparer);

    ITestSuiteBuilder<TOutput> Ignore { get; }

    ITestSuiteReporter<TOutput> Run(params int[] testNumbers);
}