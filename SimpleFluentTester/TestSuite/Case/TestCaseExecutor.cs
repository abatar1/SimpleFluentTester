using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

internal sealed class TestCaseExecutor
{
    /// <summary>
    /// Executes <see cref="TestCase"/> which means invokes its operation with given input values.
    /// </summary>
    public IComparedObject Execute(TestCase testCase, Stopwatch stopwatch)
    {
        var operation = testCase.OperationFactory.Invoke();
        
        if (operation == null)
        {
            testCase.AddValidation(ValidationResult.NonValid(ValidationSubject.Operation, "Operation not specified."));
            return new NullObject();
        }
        
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
            testCase.AddValidation(ValidationResult.Failed(ValidationSubject.Operation, e, message));
            result = e;
        }
        
        return ComparedObjectFactory.Wrap(result);
    }
}