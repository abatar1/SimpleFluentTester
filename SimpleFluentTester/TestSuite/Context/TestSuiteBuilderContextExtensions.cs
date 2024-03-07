using System;
using System.Collections.Generic;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite.Context;

public static class TestSuiteBuilderContextExtensions
{
    public static ITestSuiteBuilderContext<TOutput> WithOperation<TOutput>(
        this ITestSuiteBuilderContext<TOutput> context,
        Delegate operation)
    {
        return new TestSuiteBuilderContext<TOutput>(
            context.Number,
            context.Name,
            context.EntryAssemblyProvider,
            context.Activator,
            context.TestCases,
            operation,
            context.Comparer,
            context.Validations,
            context.ShouldBeExecuted,
            context.OutputUnderlyingType);
    }
    
    public static ITestSuiteBuilderContext<TOutput> WithDisplayName<TOutput>(
        this ITestSuiteBuilderContext<TOutput> context,
        string displayName)
    {
        return new TestSuiteBuilderContext<TOutput>(
            context.Number,
            displayName,
            context.EntryAssemblyProvider,
            context.Activator,
            context.TestCases,
            context.Operation,
            context.Comparer,
            context.Validations,
            context.ShouldBeExecuted,
            context.OutputUnderlyingType);
    }
    
    public static ITestSuiteBuilderContext<TNewOutput> ConvertType<TOutput, TNewOutput>(
        this ITestSuiteBuilderContext<TOutput> context,
        IList<TestCase<TNewOutput>> testCases,
        Func<TNewOutput?, TNewOutput?, bool>? comparer)
    {
        return new TestSuiteBuilderContext<TNewOutput>(
            context.Number,
            context.Name,
            context.EntryAssemblyProvider,
            context.Activator,
            testCases,
            context.Operation,
            comparer,
            context.Validations,
            context.ShouldBeExecuted,
            context.OutputUnderlyingType);
    }
    
    public static ITestSuiteBuilderContext<TOutput> DoNotExecute<TOutput>(
        this ITestSuiteBuilderContext<TOutput> context)
    {
        return new TestSuiteBuilderContext<TOutput>(
            context.Number,
            context.Name,
            context.EntryAssemblyProvider,
            context.Activator,
            context.TestCases,
            context.Operation,
            context.Comparer,
            context.Validations,
            false,
            context.OutputUnderlyingType);
    }
}