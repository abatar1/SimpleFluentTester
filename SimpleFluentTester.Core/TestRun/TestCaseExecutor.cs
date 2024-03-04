using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestRun;

internal sealed class TestCaseExecutor<TOutput>(TestSuiteBuilderContext<TOutput> context)
{
    public CompletedTestCase<TOutput> TryCompeteTestCase(TestCase<TOutput> testCase, SortedSet<int> testNumbersHash)
    {
        if (!ShouldBeExecuted(testCase, testNumbersHash))
            return CompletedTestCase<TOutput>.NotExecuted(testCase);

        var validationResults = testCase.Validators
            .Select(x => x.Invoke())
            .ToList();

        var valid = validationResults.All(x => x.IsValid);

        Assert<TOutput>? assert = null;
        if (valid)
            assert = ExecuteAssert(testCase);

        var validationStatus = valid switch
        {
            true => ValidationStatus.Valid,
            false => ValidationStatus.NonValid
        };
        return new CompletedTestCase<TOutput>(assert, validationStatus, testCase, validationResults);
    }

    private Assert<TOutput> ExecuteAssert(TestCase<TOutput> testCase)
    {
        var stopwatch = new Stopwatch();
        object? invokeResult;
        try
        {
            stopwatch.Start();
            invokeResult = context.Operation.Value!.Method.Invoke(context.Operation.Value.Target, testCase.Inputs);
            stopwatch.Stop();
        }
        catch (TargetInvocationException e)
        {
            return new Assert<TOutput>(false, null, e.InnerException, stopwatch.Elapsed);
        }

        TOutput? output;
        try
        {
            output = (TOutput?)invokeResult;
        }
        catch (Exception e)
        {
            return new Assert<TOutput>(false, null, e, stopwatch.Elapsed);
        }

        bool passed;
        if (context.OutputUnderlyingType != null && testCase.Expected == null)
        {
            passed = invokeResult == (object?)testCase.Expected;
        }
        else
        {
            try
            {
                passed = context.Comparer?.Invoke(output, testCase.Expected) ??
                         output?.Equals(testCase.Expected) ?? false;
            }
            catch (TargetInvocationException e)
            {
                return new Assert<TOutput>(false, null, e.InnerException, stopwatch.Elapsed);
            }
        }

        return new Assert<TOutput>(passed, new ValueWrapper<TOutput>(output), null, stopwatch.Elapsed);
    }

    private bool ShouldBeExecuted(TestCase<TOutput> testCase, SortedSet<int> testNumbersHash)
    {
        if (testNumbersHash.Count == 0)
            return true;
        if (testNumbersHash.Count != 0)
            return testNumbersHash.Max > context.TestCases.Count || testNumbersHash.Contains(testCase.Number);

        throw new Exception("This code shouldn't be reached.");
    }
}