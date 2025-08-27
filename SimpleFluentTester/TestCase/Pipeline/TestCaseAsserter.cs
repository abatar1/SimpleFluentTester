using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Helpers;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.TestCase.Pipeline;

internal static class TestCaseAsserter
{
    /// <summary>
    /// Asserts given <see cref="DefinedTestCase"/> which means compares an expected result with an actual result.
    /// </summary>
    public static AssertedTestCase Assert(this ExecutedTestCase executedTestCase)
    {
        return AssertInternal(executedTestCase);
    }

    /// <summary>
    /// Marks the given <see cref="ExecutedTestCase"/> as ignored by creating an <see cref="AssertedTestCase"/>
    /// with corresponding clauses indicating an ignored result.
    /// </summary>
    /// <param name="executedTestCase">The executed test case to be marked as ignored.</param>
    /// <returns>An <see cref="AssertedTestCase"/> instance based on the provided <see cref="ExecutedTestCase"/> where all assertions are ignored.</returns>
    public static AssertedTestCase AsIgnored(this ExecutedTestCase executedTestCase)
    {
        var clauses = executedTestCase.Clauses
            .Select(clause  => new AssertedTestClause(clause.Expected, ComparedObjectFactory.Null(), AssertResult.Ignored))
            .ToList();
        return new AssertedTestCase(clauses, executedTestCase);
    }

    /// <summary>
    /// Marks the given <see cref="ExecutedTestCase"/> as failed by creating corresponding failed
    /// assertions for each clause and encapsulating them in a new <see cref="AssertedTestCase"/>.
    /// </summary>
    /// <param name="executedTestCase">The test case that has been executed and is to be marked as failed.</param>
    /// <returns>A new <see cref="AssertedTestCase"/> containing the failed assertions for all clauses.</returns>
    public static AssertedTestCase AsFailed(this ExecutedTestCase executedTestCase)
    {
        var clauses = executedTestCase.Clauses
            .Select(clause  => new AssertedTestClause(clause.Expected, ComparedObjectFactory.Null(), new AssertResult(AssertStatus.Failed)))
            .ToList();
        return new AssertedTestCase(clauses, executedTestCase);
    }

    private static AssertedTestCase AssertInternal(ExecutedTestCase executedTestCase)
    {
        var executedTestClauses = new List<AssertedTestClause>();
        
        foreach (var clause in executedTestCase.Clauses.Cast<ExecutedTestClause>())
        {
            try
            {
                var resultAssertStatus = AssertResultSingleInternal(clause);
                var assertResult = new AssertResult(resultAssertStatus);
                executedTestClauses.Add(new AssertedTestClause(clause.Expected, clause.Result, assertResult));
            }
            catch (TargetInvocationException e)
            {
                executedTestCase.AddReadyValidation(ValidationResult.Failed(ValidationSubject.Comparer, e.InnerException,"Comparer execution failed with an exception."));
                var assertResult = new AssertResult(AssertStatus.Failed, e.InnerException, e.InnerException?.Message);
                executedTestClauses.Add(new AssertedTestClause(clause.Expected, clause.Result, assertResult));
            }
            catch (Exception e)
            {
                executedTestCase.AddReadyValidation(ValidationResult.Failed(ValidationSubject.Comparer, e, $"Comparer execution failed with an exception [{e.Message}]."));
                var assertResult = new AssertResult(AssertStatus.Failed, e, e.Message);
                executedTestClauses.Add(new AssertedTestClause(clause.Expected, clause.Result, assertResult));
            }
        }

        return new AssertedTestCase(executedTestClauses, executedTestCase);
    }
    
    private static AssertStatus AssertResultSingleInternal(ExecutedTestClause clause)
    {
        bool passed;
        switch (clause.Expected.Variety)
        {
            case ComparedObjectVariety.Null:
                passed = clause.Result.Variety == ComparedObjectVariety.Null;
                break;
            case ComparedObjectVariety.Exception:
            {
                var expectedException = (Exception?) clause.Expected.Value;
                var outputException = (Exception?) clause.Result.Value;

                var equalMessage = true;
                if (!string.IsNullOrWhiteSpace(expectedException?.Message))
                    equalMessage = expectedException.Message == outputException?.Message;
                passed = equalMessage && clause.Expected.Type == clause.Result.Type;
                break;
            }
            case ComparedObjectVariety.Value:
            {
                if (clause.Result.Variety == ComparedObjectVariety.Exception)
                    return AssertStatus.NotPassedWithException;
                
                var hasSameVariety = clause.Result.Variety == clause.Expected.Variety;
                var hasSameType = clause.Result.Type == clause.Expected.Type;
                var isEqual = (bool)clause.Comparer.Method.Invoke(clause.Comparer.Target, [clause.Expected.Value, clause.Result.Value]);

                passed = hasSameVariety && hasSameType && isEqual;
                break;
            }
            case ComparedObjectVariety.Parameter:
            {
                if (clause.Result.Variety == ComparedObjectVariety.Exception)
                    return AssertStatus.NotPassedWithException;
                
                var expectedType = ObjectParameterExtractor.ExtractExpectedParameterType(clause);
                var hasSameType = clause.Result.Type == expectedType;
                var isEqual = (bool)clause.Comparer.Method.Invoke(clause.Comparer.Target, [ObjectParameterExtractor.ExtractExpectedParameterValue(clause), clause.Result.Value]);
                
                passed = hasSameType && isEqual;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return passed ? AssertStatus.Passed : AssertStatus.NotPassed;
    }
}