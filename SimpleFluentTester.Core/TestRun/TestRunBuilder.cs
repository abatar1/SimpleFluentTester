using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;

namespace SimpleFluentTester.TestRun;

public sealed class TestRunBuilder<TOutput>
{
    private readonly TestRunBuilderContext<TOutput> _context;

    internal TestRunBuilder(BaseTestRunReporterFactory reporterFactory, 
        IEntryAssemblyProvider entryAssemblyProvider,
        Func<TOutput, TOutput, bool>? comparer)
    {
        _context = new TestRunBuilderContext<TOutput>(entryAssemblyProvider, 
            new List<TestCase<TOutput>>(),
            reporterFactory, 
            new ValueWrapper<Delegate>(),
            comparer);
    }
    
    internal TestRunBuilder(TestRunBuilderContext<TOutput> context)
    {
        _context = context;
    }

    /// <summary>
    /// Specifies the expected value resulting from the execution of this test case.
    /// </summary>
    public TestCaseInputBuilder<TOutput> Expect(TOutput expected)
    {
        return new TestCaseInputBuilder<TOutput>(expected, _context);
    }
    
    /// <summary>
    /// Specifies the method that needs to be tested.
    /// </summary>
    public TestRunBuilder<TOutput> UseOperation(Delegate operation)
    {
        ValidateOperation(operation);
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
        if (_context.Operation.Value == null)
        {
            var operation = GetDelegateFromAttributedMethod(_context.EntryAssemblyProvider);
            ValidateOperation(operation);
            _context.Operation.Value = operation;
        }
        
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

    private static MethodInfo? _assemblyMethodOfTestSuite;
    private static Type? _delegateType;
    private static ConstructorInfo? _assemblyMethodClassCtorOfTestSuite;

    private static Delegate GetDelegateFromAttributedMethod(IEntryAssemblyProvider entryAssemblyProvider)
    {
        if (_assemblyMethodOfTestSuite == null)
        {
            var entryAssembly = entryAssemblyProvider.Get();
            if (entryAssembly == null)
                throw new InvalidOperationException("No entry Assembly have been found when trying to find TestSuiteDelegateAttribute definitions");
            
            var operationMembers = entryAssembly.GetTypes()
                .SelectMany(type => type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                .Where(x => Attribute.IsDefined(x, typeof(TestSuiteDelegateAttribute)))
                .ToList();
            
            if (operationMembers.Count == 0)
                throw new InvalidOperationException($"You should specify an operation first with an {nameof(TestSuiteDelegateAttribute)} attribute or using UseOperation method");
            if (operationMembers.Count > 1)
                throw new InvalidOperationException($"You defined more than one method with {nameof(TestSuiteDelegateAttribute)}");
            
            _assemblyMethodOfTestSuite = (MethodInfo)operationMembers.Single();
        }

        if (_delegateType == null)
        {
            var assemblyMethodParameterOfTestSuite = _assemblyMethodOfTestSuite.GetParameters()
                .Select(x => x.ParameterType)
                .Append(_assemblyMethodOfTestSuite.ReturnType)
                .ToArray();
            _delegateType = Expression.GetDelegateType(assemblyMethodParameterOfTestSuite);
        }

        if (_assemblyMethodOfTestSuite.IsStatic)
            return _assemblyMethodOfTestSuite.CreateDelegate(_delegateType);

        var methodClassType = _assemblyMethodOfTestSuite.DeclaringType;
        if (methodClassType == null)
            throw new InvalidOperationException("No declaring type for non-static method, should be the bug");

        if (_assemblyMethodClassCtorOfTestSuite == null)
        {
            var methodClassCtor = methodClassType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            _assemblyMethodClassCtorOfTestSuite = methodClassCtor
                .FirstOrDefault(x => x.GetParameters().Length == 0);
            if (_assemblyMethodClassCtorOfTestSuite == null)
                throw new InvalidOperationException("TestSuiteDelegateAttribute has been defined");
        }

        var target = Activator.CreateInstance(methodClassType);
        
        return _assemblyMethodOfTestSuite.CreateDelegate(_delegateType, target);
    }

    private static void ValidateOperation(Delegate operation)
    {
        if (operation.Method.ReturnParameter?.ParameterType != typeof(TOutput))
            throw new InvalidCastException($"{nameof(UseOperation)} thrown an exception, operation return type is not the same as used generic type.");
    }
}