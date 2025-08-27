using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite;

internal interface ITestSuiteController
{
    void Ignore(params int[] testSuiteNumbers);

    void Ignore(params string[] testSuiteNames);

    void Allow(params int[] testSuiteNumbers);

    void Allow(params string[] testSuiteNames);

    bool IsAllowed(ITestSuiteContext context);
}