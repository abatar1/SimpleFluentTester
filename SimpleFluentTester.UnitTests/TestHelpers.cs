using System.Reflection;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.Suite;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests
{
    public static class TestHelpers
    {
        public static TestSuiteResult<TOutput> GetTestRunResultFromReporter<TOutput>(BaseTestRunReporter<TOutput> reporter)
        {
            var baseReporterType = reporter.GetType().BaseType;
            Assert.NotNull(baseReporterType);
            var reporterFields = baseReporterType
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        
            var testRunResultProperty = reporterFields
                .FirstOrDefault(x => x.FieldType == typeof(TestSuiteResult<TOutput>));
            Assert.NotNull(testRunResultProperty);
            var testCases = testRunResultProperty.GetValue(reporter) as TestSuiteResult<TOutput>;
            Assert.NotNull(testCases);
            return testCases;
        }
    
        public static TestSuiteBuilderContext<TOutput> CreateEmptyContext<TOutput>(IEntryAssemblyProvider? assemblyProvider = null)
        {
            assemblyProvider ??= new EntryAssemblyProvider();
            return new TestSuiteBuilderContext<TOutput>(
                0,
                "TestSuite",
                assemblyProvider, 
                new DefaultActivator(),
                new List<TestCase<TOutput>>(), 
                new HashSet<ValidationInvoker<TOutput>>(),
                new DefaultTestSuiteReporterFactory(), 
                new ValueWrapper<Delegate>(), 
                null, 
                true);
        }
    }
}