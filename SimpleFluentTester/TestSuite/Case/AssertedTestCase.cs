using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

/// <summary>
/// Represents <see cref="DeferredTestCase"/> after proceeding by <see cref="TestCasePipeline"/>. Contains validation results of TestCase,
/// assertion result, elapsed execution time. Could be ignored using <see cref="AssertedTestCase.NotExecuted"/>
/// </summary>
public sealed class AssertedTestCase : ITestCase
{
    internal AssertedTestCase(AssertResult assertResult, ITestCase testCase, TimeSpan elapsedTime)
    {
        Number = testCase.Number;
        Assert = assertResult;
        ElapsedTime = elapsedTime;
        Inputs = testCase.Inputs;
        Expected = testCase.Expected;
        Validations = testCase.Validations;
    }

    public int Number { get; }
    
    public AssertResult Assert { get; }
    
    public TimeSpan ElapsedTime { get; }
    
    public IComparedObject[] Inputs { get; }

    public IComparedObject Expected { get; }

    public IDictionary<ValidationSubject, IList<Lazy<ValidationResult>>> Validations { get; }

    public bool IsValid => Validations.Values.SelectMany(x => x).All(x => x.Value.Status == ValidationStatus.Valid);
    
    public IReadOnlyCollection<ValidationResult> GetNonValid()
    {
        return Validations.Values.SelectMany(x => x)
            .Select(x => x.Value)
            .Where(x => x.Status != ValidationStatus.Valid)
            .ToList();
    }
    
    public static AssertedTestCase NotExecuted(ITestCase testCase)
    {
        return new AssertedTestCase(AssertResult.Ignored, testCase, TimeSpan.Zero); 
    }
}
