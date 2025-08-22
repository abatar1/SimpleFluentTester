using System.Collections.Generic;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase.Pipeline;

/// <summary>
/// Represents a pipeline for processing test cases within a test suite.
/// Responsible for unpacking, validating, executing, asserting test cases
/// to produce a <see cref="AssertedTestCase"/> from a <see cref="DefinedTestCase"/>.
/// </summary>
/// <remarks>
/// This class determines whether the test case should be executed based on the provided test case hash,
/// ensures the test case is valid, manages execution timing, and evaluates the results using assertions.
/// </remarks>
internal sealed class TestCasePipeline(ISet<int> testNumbersHash)
{
    /// <summary>
    /// Unpack, execute, assert test cases. 
    /// </summary>
    public ITestCase ToCompleted(DefinedTestCase deferredTestCase)
    {
        if (!ShouldBeExecuted(deferredTestCase) || deferredTestCase.Validate() != ValidationStatus.Valid)
            return deferredTestCase.AsIgnored().AsIgnored();
        
        return deferredTestCase.Execute().Assert();
    }
    
    private bool ShouldBeExecuted(DefinedTestCase testCase)
    {
        return testNumbersHash.Count == 0 || testNumbersHash.Contains(testCase.Number); 
    }
}