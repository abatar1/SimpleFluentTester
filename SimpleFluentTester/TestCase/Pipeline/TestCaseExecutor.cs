using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Helpers;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.TestCase.Pipeline;

/// <summary>
/// Represents a class that executes a test case.
/// </summary>
/// <remarks>
/// This class invokes the operation with the given input values and returns the result.
/// </remarks>
internal static class TestCaseExecutor
{
    private static readonly Delegate EmptyDelegate = new Action(() => { });
    
    /// <summary>
    /// Executes <see cref="DefinedTestCase"/> which means invokes its operation with given input values.
    /// </summary>
    public static ExecutedTestCase Execute(this DefinedTestCase testCase)
    {
        var stopwatch = new Stopwatch();

        var operation = GetOperation(testCase);
        
        object? operationResult;
        try
        {
            stopwatch.Start();
            operationResult = operation.Method.Invoke(operation.Target, testCase.Inputs.Select(x => x.Value).ToArray());
            stopwatch.Stop();
        }
        catch (TargetInvocationException e)
        {
            operationResult = e.InnerException;
        }
        catch (Exception e)
        {
            const string message = "Couldn't invoke operation, possibly input and operation parameters do not match and pre-validation has failed and seems like a bug.";
            testCase.AddReadyValidation(ValidationResult.Failed(ValidationSubject.Operation, e, message));
            operationResult = e;
        }

        var clauses = GetExecutedClauses(testCase, operationResult);
        return new ExecutedTestCase(clauses, stopwatch.Elapsed, testCase);
    }

    /// <summary>
    /// Marks the given <see cref="DefinedTestCase"/> as ignored by creating an <see cref="ExecutedTestCase"/>
    /// with preset values indicating no operation was performed.
    /// </summary>
    /// <param name="testCase">The <see cref="DefinedTestCase"/> to be marked as ignored.</param>
    /// <returns>An <see cref="ExecutedTestCase"/> instance representing the ignored test case with no execution performed.</returns>
    public static ExecutedTestCase AsNotExecuted(this DefinedTestCase testCase)
    {
        var clauses = testCase.Clauses
            .Select(clause => new ExecutedTestClause(EmptyDelegate, clause.Expected, ComparedObjectFactory.Null()))
            .ToList();
        return new ExecutedTestCase(clauses, TimeSpan.Zero, testCase);
    }

    private static List<ExecutedTestClause> GetExecutedClauses(DefinedTestCase testCase, object? operationResult)
    {
        return testCase.Clauses
            .Select(testClause =>
            {
                IComparedObject result;
                if (testClause.Expected.Variety == ComparedObjectVariety.Parameter)
                {
                    var parameterInfo = ObjectParameterExtractor.ExtractExpectedParameter(testClause);
                    // We get actual value from parameters of the function after execution.
                    result = testCase.Inputs[parameterInfo.Position];
                }
                else
                {
                    result = ComparedObjectFactory.Wrap(operationResult);
                } 
                var comparer = GetClauseComparer(testClause, testCase);
                return new ExecutedTestClause(comparer, testClause.Expected, result);
            })
            .ToList();
    }
    
    private static Delegate GetClauseComparer(ITestClause testClause, DefinedTestCase testCase)
    {
        var clauseExpectedType = GetClauseExpectedType(testClause);

        if (testCase.ComparerFactory.Value != null)
        {
            var expectedParameterType = testCase.ComparerFactory.Value.Method.GetParameters()[0].ParameterType;
            if (expectedParameterType != clauseExpectedType)
                throw new InvalidContextException("Could not find comparer for expected type " + clauseExpectedType + " and actual type " + expectedParameterType + ".");
            return testCase.ComparerFactory.Value;
        }

        return GetComparerDelegate(clauseExpectedType);
    }

    private static Delegate GetOperation(DefinedTestCase testCase)
    {
        return testCase.OperationFactory.Value ?? throw new InvalidContextException("Operation factory is null after validation, seems like a bug.");
    }

    private static Type? GetClauseExpectedType(ITestClause testClause)
    {
        if (testClause.Expected.Variety == ComparedObjectVariety.Parameter)
        {
            return ObjectParameterExtractor.ExtractExpectedParameterType(testClause);
        }
        return testClause.Expected.Type;
    }

    private static Delegate GetComparerDelegate(Type? type)
    {
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