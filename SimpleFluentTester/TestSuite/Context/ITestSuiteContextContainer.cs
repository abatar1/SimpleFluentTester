using System;
using SimpleFluentTester.Helpers;

namespace SimpleFluentTester.TestSuite.Context;

internal interface ITestSuiteContextContainer
{
    ITestSuiteContext Context { get; }

    void WithOperation(Delegate operation);

    void WithDisplayName(string displayName);

    void WithComparer<TExpected>(ComparerDelegate<TExpected> comparer);

    void WithEntryAssemblyProvider(IEntryAssemblyProvider entryAssemblyProvider);

    void WithActivator(IActivator activator);

    void DoNotExecute();
}