using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Starting point used to initialize tests.
/// </summary>
public static class TestSuite
{
    private static int _testSuiteNumber;
        
    public static ITestSuiteBuilder<object> Sequential
    {
        get
        {
            _testSuiteNumber += 1;
            var defaultContext = TestSuiteBuilderContext<object>.Default(_testSuiteNumber);
            return new TestSuiteBuilder<object>(defaultContext);
        }
    }
}