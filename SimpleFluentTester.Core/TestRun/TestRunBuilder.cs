using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.TestRun;

public sealed class TestRunBuilder<TOutput>
{
    private readonly TestRunBuilderContext<TOutput> _context;

    internal TestRunBuilder(BaseTestRunReporterFactory reporterFactory, 
        IEntryAssemblyProvider entryAssemblyProvider,
        IActivator activator,
        Func<TOutput?, TOutput?, bool>? comparer,
        bool shouldBeExecuted = true)
    {
        _context = new TestRunBuilderContext<TOutput>(entryAssemblyProvider, 
            activator,
            new List<TestCase<TOutput>>(),
            reporterFactory, 
            new ValueWrapper<Delegate>(),
            comparer,
            shouldBeExecuted);
    }
    
    internal TestRunBuilder(TestRunBuilderContext<TOutput> context)
    {
        _context = context;
    }

    /// <summary>
    /// Specifies the expected value resulting from the execution of this test case.
    /// </summary>
    public TestCaseInputBuilder<TOutput> Expect(TOutput? expected)
    {
        if (_context.IsObjectOutput)
            ValidateOperation(_context, expected?.GetType());
        
        return new TestCaseInputBuilder<TOutput>(expected, _context);
    }
    
    /// <summary>
    /// Specifies the method that needs to be tested.
    /// </summary>
    public TestRunBuilder<TOutput> UseOperation(Delegate operation)
    {
        _context.Operation.Value = operation;
        return this;
    }
    
    /// <summary>
    /// Allows defining a custom reporter that enables determining a custom report format.
    /// </summary>
    public TestRunBuilder<TOutput> WithCustomReporterFactory<TReporterFactory>()
        where TReporterFactory : BaseTestRunReporterFactory
    {
        _context.ReporterFactory = (TReporterFactory)Activator.CreateInstance(typeof(TReporterFactory));
        return this;
    }
    
    /// <summary>
    /// Initiates the execution of test cases defined earlier using <see cref="Expect"/>.
    /// For debugging failed test cases, it also allows selecting the test case numbers that should be executed, all others will be skipped.
    /// </summary>
    public BaseTestRunReporter<TOutput> Run(params int[] testNumbers)
    {
        if (!_context.ShouldBeExecuted)
            return new EmptyTestRunReporter<TOutput>(_context.TestCases);
        
        _context.Operation.Value ??= TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(_context.EntryAssemblyProvider, _context.Activator);

        if (!_context.IsObjectOutput)
            ValidateOperation(_context, typeof(TOutput));
        ValidateComparer(_context);
        
        var testNumbersHash = new HashSet<int>(testNumbers);
        
        if (testNumbersHash.Count != 0 && (testNumbersHash.Count > _context.TestCases.Count || testNumbersHash.Max() > _context.TestCases.Count))
            throw new InvalidOperationException("Invalid test case numbers were given as input");

        var executedTestCases = _context.TestCases
            .Select((testResult, it) => (testResult, it + 1))
            .Select(x =>
            {
                var shouldBeCalculated = testNumbersHash.Count == 0 || (testNumbersHash.Count != 0 && testNumbersHash.Contains(x.Item2));
                if (shouldBeCalculated)
                    _ = x.testResult.LazyResult.Value;
                return x.testResult with { ShouldBeCalculated = shouldBeCalculated };
            })
            .ToList();

        return (BaseTestRunReporter<TOutput>)_context.ReporterFactory.GetReporter<TOutput>(executedTestCases, _context.Operation.Value.Method);
    }

    private static void ValidateOperation(TestRunBuilderContext<TOutput> context, Type? actualOperationType)
    {
        if (context.Operation.Value == null)
            throw new InvalidCastException("Operation haven't been specified, please use UseOperation method");

        if (context.Operation.Value.Method.ReturnParameter == null)
            throw new InvalidOperationException("Operation must have return type to be testable");

        Type? returnParameterType;

        if (context.OutputUnderlyingType != null)
        {
            if (actualOperationType == null)
                return;
            returnParameterType = context.OutputUnderlyingType;
        }
        else
        {
            returnParameterType = context.Operation.Value.Method.ReturnParameter.ParameterType;
        }
        
        if (returnParameterType != actualOperationType)
            throw new InvalidCastException("Operation return type is not the same as used generic type.");
    }

    private static void ValidateComparer(TestRunBuilderContext<TOutput> context)
    {
        if (context.Comparer != null || context.IsObjectOutput)
            return;
        
        if (!typeof(IEquatable<TOutput>).IsAssignableFrom(typeof(TOutput)))
            throw new InvalidOperationException("TOutput type should be assignable from IEquatable<TOutput> or comparer should be defined");
    }
}
