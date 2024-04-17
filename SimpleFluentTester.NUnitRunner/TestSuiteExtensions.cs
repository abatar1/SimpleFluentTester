using NUnitLite;
using SimpleFluentTester.NUnitRunner.InternalTests;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.NUnitRunner;

public static class TestSuiteExtensions
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="reporter"></param>
    /// <param name="configEnricher"></param>
    public static void ReportNUnit(this ITestSuiteReporter<object> reporter, Action<NUnitLiteConfiguration>? configEnricher = null)
    {
        var configuration = new NUnitLiteConfiguration();
        configEnricher?.Invoke(configuration);
        
        TestCaseTests.Datapoint = reporter.TestSuiteResult.TestCases
            .Select(x => x)
            .ToArray();
        ContextValidationTests.ValidationResult = reporter.TestSuiteResult.ContextValidation;

        var configBuilder = new NUnitLiteConfigurationBuilder(configuration);
        
        new AutoRun().Execute(configBuilder.Build());
    }
}