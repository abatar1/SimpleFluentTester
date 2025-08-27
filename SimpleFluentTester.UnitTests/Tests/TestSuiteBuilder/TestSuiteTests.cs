using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

[Collection("NonParallelCollection")]
public sealed class TestSuiteTests
{
    [Fact]
    public void TestSuiteGlobalState_AllowByName_ShouldPass()
    {
        // Assign
        var name = "TestName1";
        var container = TestSuiteContextContainer.Default();
        container.WithDisplayName(name);
        container.WithOperation((int x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue([1], 1);
        var builder = new SequentialTestSuiteBuilder(container, testCase.Clauses.Cast<DefinedTestClause>().ToList());
        
        // Act
        TestSuite.TestSuite.WithController(new TestSuiteController());
        TestSuite.TestSuite.Allow(name);
        var reporter = builder.Run();
        TestSuite.TestSuite.WithController(new TestSuiteController());

        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed(1, [1]);
    }
    
    [Fact]
    public void TestSuiteGlobalState_AllowByNumber_ShouldPass()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue([1], 1);
        var builder = new SequentialTestSuiteBuilder(container, testCase.Clauses.Cast<DefinedTestClause>().ToList());
        
        // Act
        TestSuite.TestSuite.WithController(new TestSuiteController());
        TestSuite.TestSuite.Allow(1);
        var reporter = builder.Run();
        TestSuite.TestSuite.WithController(new TestSuiteController());

        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed(1, [1]);
    }
    
    [Fact]
    public void TestSuiteGlobalState_IgnoreByNumber_ShouldBeIgnored()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue([1], 1);
        var builder = new SequentialTestSuiteBuilder(container, testCase.Clauses.Cast<DefinedTestClause>().ToList());
        
        // Act
        TestSuite.TestSuite.WithController(new TestSuiteController());
        TestSuite.TestSuite.Ignore(1);
        var reporter = builder.Run();
        TestSuite.TestSuite.WithController(new TestSuiteController());

        // Assert
        reporter.AssertTestCaseExists(1).AssertIgnored();
    }
    
    [Fact]
    public void TestSuiteGlobalState_IgnoreByName_ShouldBeIgnored()
    {
        // Assign
        var name = "TestName2";
        var container = TestSuiteContextContainer.Default();
        container.WithDisplayName(name);
        container.WithOperation((int x) => x);
        var testCase = container.DefineTestCaseWithExpectedValue([1], 1);
        var builder = new SequentialTestSuiteBuilder(container, testCase.Clauses.Cast<DefinedTestClause>().ToList());
        
        // Act
        TestSuite.TestSuite.WithController(new TestSuiteController());
        TestSuite.TestSuite.Ignore(name);
        var reporter = builder.Run();
        TestSuite.TestSuite.WithController(new TestSuiteController());

        // Assert
        reporter.AssertTestCaseExists(1).AssertIgnored();
    }
}