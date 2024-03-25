using System;

namespace SimpleFluentTester.TestSuite.Context;

public interface ITestSuiteContextContainer
{
    ITestSuiteContext Context { get; }

    void WithOperation(Delegate operation);

    void WithDisplayName(string displayName);

    void WithComparer(Delegate comparer);

    void DoNotExecute();
}