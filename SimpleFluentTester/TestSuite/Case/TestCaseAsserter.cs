using System;
using System.Reflection;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

internal sealed class TestCaseAsserter
{
    /// <summary>
    /// Asserts given <see cref="TestCase"/> which means compares expected result with an actual result.
    /// </summary>
    public AssertResult Assert(TestCase testCase, IComparedObject output)
    {
        var comparer = testCase.ComparerFactory.Invoke();
        
        try
        {
            var assertStatus = AssertInternal(output, testCase.Expected, comparer);
            return new AssertResult(output, assertStatus);
        }
        catch (TargetInvocationException e)
        {
            testCase.AddValidation(ValidationResult.NonValid(ValidationSubject.Comparer, "Comparer execution failed with an exception."));
            return new AssertResult(output, AssertStatus.Failed, e.InnerException, e.InnerException?.Message);
        }
        catch (Exception e)
        {
            testCase.AddValidation(ValidationResult.NonValid(ValidationSubject.Comparer, $"Comparer execution failed with an exception [{e.Message}]."));
            return new AssertResult(output, AssertStatus.Failed, e, e.Message);
        }
    }

    private static AssertStatus AssertInternal(IComparedObject output, IComparedObject expected, Delegate? comparer)
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
                    equalMessage = expectedException?.Message == outputException?.Message;
                passed = equalMessage && expected.Type == output.Type;
                break;
            }
            case ComparedObjectVariety.Value:
            {
                var hasSameVariety = output.Variety == expected.Variety;
                var hasSameType = output.Type == expected.Type;
                
                var valueObject = (ValueObject)output;
                
                bool isEqual;
                if (comparer != null)
                    isEqual = (bool)comparer.Method.Invoke(comparer.Target, [output.Value, expected.Value]);
                else
                    isEqual = valueObject.Value.Equals(expected.Value);

                passed = hasSameVariety && hasSameType && isEqual;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return passed ? AssertStatus.Passed : AssertStatus.NotPassed;
    }
}