using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests;

public static class TestHelpers
{
    public static TestRunResult<TOutput> GetTestRunResultFromReporter<TOutput>(BaseTestRunReporter<TOutput> reporter)
    {
        var baseReporterType = reporter.GetType().BaseType;
        Assert.NotNull(baseReporterType);
        var reporterFields = baseReporterType
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        
        var testRunResultProperty = reporterFields
            .FirstOrDefault(x => x.FieldType == typeof(TestRunResult<TOutput>));
        Assert.NotNull(testRunResultProperty);
        var testCases = testRunResultProperty.GetValue(reporter) as TestRunResult<TOutput>;
        Assert.NotNull(testCases);
        return testCases;
    }
    
    public static TestRunBuilderContext<TOutput> CreateEmptyContext<TOutput>(IEntryAssemblyProvider? assemblyProvider = null)
    {
        assemblyProvider ??= new EntryAssemblyProvider();
        return new TestRunBuilderContext<TOutput>(
            assemblyProvider, 
            new DefaultActivator(),
            new List<TestCase<TOutput>>(), 
            new HashSet<ValidationInvoker<TOutput>>(),
            new DefaultTestRunReporterFactory(), 
            new ValueWrapper<Delegate>(), 
            null, 
            true);
    }
}