using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

internal sealed class TestCasePipeline(ISet<int> testNumbersHash)
{
    private readonly TestCaseAsserter _testCaseAsserter = new();
    private readonly TestCaseExecutor _testCaseExecutor = new();

    /// <summary>
    /// Unpack, execute and assert test cases. Produces <see cref="CompletedTestCase"/> from <see cref="TestCase"/>.
    /// </summary>
    public CompletedTestCase ToCompleted(TestCase testCase)
    {
        if (!ShouldBeExecuted(testCase))
            return CompletedTestCase.NotExecuted(testCase);
        
        var packedValidation = ValidationPipe.ValidatePacked(testCase);
        
        if (!packedValidation.IsValid)
            return new CompletedTestCase(AssertResult.Ignored, TimeSpan.Zero, packedValidation, testCase); 
        
        var stopwatch = new Stopwatch();
        var output = _testCaseExecutor.Execute(testCase, stopwatch);
        var assertResult = _testCaseAsserter.Assert(testCase, output);
        return new CompletedTestCase(assertResult, stopwatch.Elapsed, packedValidation, testCase);
    }
    
    private bool ShouldBeExecuted(TestCase testCase)
    {
        return testNumbersHash.Count == 0 || testNumbersHash.Contains(testCase.Number);
    }
}