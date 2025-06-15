using System;
using System.Reflection;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

internal static class TestCaseAsserter
{
    /// <summary>
    /// Asserts given <see cref="DeferredTestCase"/> which means compares an expected result with an actual result.
    /// </summary>
    public static AssertedTestCase Assert(this ExecutedTestCase executedTestCase)
    {
        AssertResult assertResult;
        try
        {
            var assertStatus = AssertInternal(executedTestCase.Result, executedTestCase.Expected, executedTestCase.Comparer);
            assertResult = new AssertResult(executedTestCase.Result, assertStatus);
        }
        catch (TargetInvocationException e)
        {
            executedTestCase.AddReadyValidation(ValidationResult.NonValid(ValidationSubject.Comparer, "Comparer execution failed with an exception."));
            assertResult = new AssertResult(executedTestCase.Result, AssertStatus.Failed, e.InnerException, e.InnerException?.Message);
        }
        catch (Exception e)
        {
            executedTestCase.AddReadyValidation(ValidationResult.NonValid(ValidationSubject.Comparer, $"Comparer execution failed with an exception [{e.Message}]."));
            assertResult = new AssertResult(executedTestCase.Result, AssertStatus.Failed, e, e.Message);
        }
        return BuildAsserted(assertResult, executedTestCase);
    }

    private static AssertedTestCase BuildAsserted(AssertResult assertResult, ExecutedTestCase executedTestCase)
    {
        return new AssertedTestCase(assertResult, executedTestCase, executedTestCase.ElapsedTime);
    }

    private static AssertStatus AssertInternal(IComparedObject output, IComparedObject expected, Delegate comparer)
    {
        bool passed;
        switch (output.Variety)
        {
            case ComparedObjectVariety.Null:
                passed = expected.Variety == ComparedObjectVariety.Null;
                break;
            case ComparedObjectVariety.Exception when expected.Variety != ComparedObjectVariety.Exception:
                return AssertStatus.NotPassedWithException;
            case ComparedObjectVariety.Exception:
            {
                var expectedException = (Exception?) expected.Value;
                var outputException = (Exception?) output.Value;

                var equalMessage = true;
                if (!string.IsNullOrWhiteSpace(expectedException?.Message))
                    equalMessage = expectedException.Message == outputException?.Message;
                passed = equalMessage && expected.Type == output.Type;
                break;
            }
            case ComparedObjectVariety.Value:
            {
                var hasSameVariety = output.Variety == expected.Variety;
                var hasSameType = output.Type == expected.Type;
                
                var isEqual = (bool)comparer.Method.Invoke(comparer.Target, [output.Value, expected.Value]);

                passed = hasSameVariety && hasSameType && isEqual;
                break;
            }
            case ComparedObjectVariety.Parameter:
                throw new NotSupportedException("Something went wrong, parameter is not supposed to be asserted and converted before assertion.");
            default:
                throw new ArgumentOutOfRangeException();
        }

        return passed ? AssertStatus.Passed : AssertStatus.NotPassed;
    }
}