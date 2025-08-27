using System;
using System.Collections.Generic;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.TestSuite;

/// <summary>
/// Represents the result of a test suite execution, including details of the executed test cases,
/// validations performed, and additional metadata about the test suite.
/// </summary>
public interface ITestSuiteRunResult
{
    bool ShouldBeExecuted { get; }

    bool IsValid { get; }

    Exception? Exception { get; }
    
    IList<AssertedTestCase> TestCases { get; }

    Delegate? Operation { get; }

    string? DisplayName { get; }

    int Number { get; }
}