using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

/// <summary>
/// Represents a class that executes a test case.
/// </summary>
/// <remarks>
/// This class invokes the operation with the given input values and returns the result.
/// </remarks>
internal static class TestCaseExecutor
{
    /// <summary>
    /// Executes <see cref="DeferredTestCase"/> which means invokes its operation with given input values.
    /// </summary>
    public static ExecutedTestCase Execute(DeferredTestCase testCase)
    {
        var stopwatch = new Stopwatch();

        var operation = GetOperation(testCase);
        
        object? result;
        try
        {
            stopwatch.Start();
            result = operation.Method.Invoke(operation.Target, testCase.Inputs.Select(x => x.Value).ToArray());
            stopwatch.Stop();
        }
        catch (TargetInvocationException e)
        {
            result = e.InnerException;
        }
        catch (Exception e)
        {
            const string message = "Couldn't invoke operation, possibly input and operation parameters do not match and pre-validation has failed and seems like a bug.";
            testCase.AddReadyValidation(ValidationResult.Failed(ValidationSubject.Operation, e, message));
            result = e;
        }
        
        return new ExecutedTestCase(GetResult(testCase, result), GetExpected(testCase), stopwatch.Elapsed, GetComparer(testCase), testCase, testCase.Validations);
    }

    private static IComparedObject GetResult(ITestCase testCase, object? result)
    {
        if (testCase.Expected.Variety == ComparedObjectVariety.Parameter)
        {
            var parameterInfo = ObjectParameterExtractor.ExtractExpectedParameter(testCase);
            return testCase.Inputs[parameterInfo.Position];
        }

        return ComparedObjectFactory.Wrap(result);
    }

    private static IComparedObject GetExpected(ITestCase testCase)
    {
        if (testCase.Expected.Variety == ComparedObjectVariety.Parameter)
        {
            return testCase.Expected.Value as IComparedObject ?? throw new InvalidOperationException("Seems like a bug that value of expected parameter is not an IComparedObject");
        }

        return testCase.Expected;
    }
    
    private static Delegate GetComparer(DeferredTestCase testCase)
    {
        var expectedType = GetExpectedType(testCase);
        var comparer = testCase.ComparerFactory.Value;
        if (comparer == null)
            return GetEqualityDelegate(expectedType);
        return comparer;
    }

    private static Delegate GetOperation(DeferredTestCase testCase)
    {
        return testCase.OperationFactory.Value ?? throw new ArgumentNullException(nameof(testCase.OperationFactory), "Operation factory is null after validation, seems like a bug.");
    }

    private static Type? GetExpectedType(DeferredTestCase testCase)
    {
        if (testCase.Expected.Variety == ComparedObjectVariety.Parameter)
            return ObjectParameterExtractor.ExtractExpectedParameterType(testCase);
        return testCase.Expected.Type;
    }

    private static Delegate GetEqualityDelegate(Type? type)
    {
        
        
        if (type == null)
            return new Func<object?, object?, bool>(Equals);
        
        if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            return new Func<object?, object?, bool>((x, y) =>
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;

                var xEnum = ((IEnumerable)x).Cast<object>();
                var yEnum = ((IEnumerable)y).Cast<object>();

                return xEnum.SequenceEqual(yEnum);
            });
        }
        
        return new Func<object?, object?, bool>(Equals);
    }
}