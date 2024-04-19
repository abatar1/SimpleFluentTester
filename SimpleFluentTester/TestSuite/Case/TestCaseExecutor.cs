using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

internal sealed class TestCaseExecutor(ITestSuiteContext context, IComparedObjectFactory comparedObjectFactory)
{
    public IComparedObject Execute(TestCase testCase, Stopwatch stopwatch)
    {
        if (context.Operation == null)
        {
            testCase.AddValidation(ValidationResult.NonValid(ValidationSubject.Operation, "Operation not specified."));
            return new NullObject();
        }
        
        object? result;
        try
        {
            stopwatch.Start();
            result = context.Operation.Method.Invoke(context.Operation.Target, testCase.Inputs.Select(x => x.Value).ToArray());
            stopwatch.Stop();
        }
        catch (TargetInvocationException e)
        {
            result = e.InnerException;
        }
        catch (Exception e)
        {
            const string message = "Couldn't invoke operation, possibly input and operation parameters do not match";
            testCase.AddValidation(ValidationResult.Failed(ValidationSubject.Operation, e, message));
            result = e;
        }
        
        return comparedObjectFactory.Wrap(result);
    }
}