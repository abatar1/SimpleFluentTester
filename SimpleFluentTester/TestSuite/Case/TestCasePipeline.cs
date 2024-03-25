using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

internal sealed class TestCasePipeline(
    ITestSuiteContext context,
    IComparedObjectFactory comparedObjectFactory,
    IValidationUnpacker validationUnpacker,
    ISet<int> testNumbersHash)
{
    private readonly TestCaseAffirmer _testCaseAffirmer = new(context);
    private readonly TestCaseExecutor _testCaseExecutor = new(context, comparedObjectFactory);

    public CompletedTestCase ToCompleted(TestCase testCase)
    {
        if (!ShouldBeExecuted(testCase))
            return CompletedTestCase.NotExecuted(testCase);
        
        var validationUnpacked = validationUnpacker.Unpack(testCase);
        
        if (!validationUnpacked.IsValid)
            return new CompletedTestCase(AssertResult.Ignored, TimeSpan.Zero, validationUnpacked, testCase); 
        
        var stopwatch = new Stopwatch();
        var output = _testCaseExecutor.Execute(testCase, stopwatch);
        var assertResult = _testCaseAffirmer.Assert(testCase, output);
        return new CompletedTestCase(assertResult, stopwatch.Elapsed, validationUnpacked, testCase);
    }
    
    private bool ShouldBeExecuted(TestCase testCase)
    {
        return testNumbersHash.Count == 0 || testNumbersHash.Contains(testCase.Number);
    }
}