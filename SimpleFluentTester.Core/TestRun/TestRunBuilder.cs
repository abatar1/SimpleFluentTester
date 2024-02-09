using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.TestRun;

public sealed class TestRunBuilder<TOutput>
{
    private readonly BaseTestRunReporterFactory _reporterFactory;
    private readonly Delegate _operation;
    private readonly IList<TestCase<TOutput>> _testCases;
    private readonly ParameterInfo[] _operationParameters;

    internal TestRunBuilder(Delegate operation, BaseTestRunReporterFactory reporterFactory)
    {
        _reporterFactory = reporterFactory ?? throw new ArgumentNullException(nameof(reporterFactory));
        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        _operationParameters = _operation.Method.GetParameters();
        _testCases = new List<TestCase<TOutput>>();
    }
    
    internal TestRunBuilder(Delegate operation, 
        ParameterInfo[] operationParameters, 
        BaseTestRunReporterFactory reporterFactory, 
        IList<TestCase<TOutput>> testCases)
    {
        _reporterFactory = reporterFactory ?? throw new ArgumentNullException(nameof(reporterFactory));
        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        _operationParameters = operationParameters ?? throw new ArgumentNullException(nameof(operationParameters));
        _testCases = testCases ?? throw new ArgumentNullException(nameof(testCases));
    }

    public TestCaseInputBuilder<TOutput> Expect(TOutput expected)
    {
        return new TestCaseInputBuilder<TOutput>(expected, _testCases, _reporterFactory, _operation, _operationParameters);
    }
    
    public BaseTestRunReporter<TOutput> Run(params int[] testNumbers)
    {
        var testNumbersHash = new HashSet<int>(testNumbers);
        
        if (testNumbersHash.Count != 0 && (testNumbersHash.Count > _testCases.Count || testNumbersHash.Max() > _testCases.Count))
            throw new InvalidOperationException("Invalid test case numbers were given as input");

        var executedTestCases = _testCases
            .Select((testResult, it) => (testResult, it + 1))
            .Select(x =>
            {
                var shouldBeCalculated = testNumbersHash.Count == 0 || (testNumbersHash.Count != 0 && testNumbersHash.Contains(x.Item2));
                if (shouldBeCalculated)
                    _ = x.testResult.LazyResult.Value;
                return x.testResult with { ShouldBeCalculated = shouldBeCalculated };
            })
            .ToList();

        return (BaseTestRunReporter<TOutput>)_reporterFactory.GetReporter<TOutput>(executedTestCases, _operation.Method);
    }
}