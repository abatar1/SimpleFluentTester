using System.Collections.Generic;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

/// <summary>
/// Represents a pipeline for processing test cases within a test suite.
/// Responsible for unpacking, validating, executing, and asserting test cases
/// to produce a <see cref="AssertedTestCase"/> from a <see cref="DeferredTestCase"/>.
/// </summary>
/// <remarks>
/// This class determines whether the test case should be executed based on the provided test case hash,
/// ensures the test case is valid, manages execution timing, and evaluates the results using assertions.
/// </remarks>
internal sealed class TestCasePipeline(ISet<int> testNumbersHash)
{
    /// <summary>
    /// Unpack, execute and assert test cases. Produces <see cref="AssertedTestCase"/> from <see cref="DeferredTestCase"/>.
    /// </summary>
    public AssertedTestCase ToCompleted(DeferredTestCase deferredTestCase)
    {
        if (!ShouldBeExecuted(deferredTestCase))
            return AssertedTestCase.NotExecuted(deferredTestCase);

        if (!deferredTestCase.Validate())
            return AssertedTestCase.NotExecuted(deferredTestCase);
        
        return TestCaseExecutor.Execute(deferredTestCase).Assert();
    }
    
    private bool ShouldBeExecuted(DeferredTestCase testCase)
    {
        return testNumbersHash.Count == 0 || testNumbersHash.Contains(testCase.Number); 
    }
}