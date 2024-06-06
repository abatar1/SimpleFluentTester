using System.Diagnostics;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.TestObjects;

namespace SimpleFluentTester.UnitTests.Tests;

public sealed class TestCaseExecutorTests
{
    [Fact]
    public void Execute_ValidExecution_ShouldReturnValue()
    {
        // Assign
        const int expectedResult = 3;
        var input = new[] { 1, 2 }.Cast<object?>().ToArray();
        
        var container = TestSuiteFactory.CreateEmptyContextContainer(operation: (int x, int y) => x + y);

        var testCaseExecutor = new TestCaseExecutor();
        
        var testCase = TestSuiteFactory.CreateAndAddTestCase(container, input, expectedResult);
        var sw = new Stopwatch();
        
        // Act
        var result = testCaseExecutor.Execute(testCase, sw);

        // Assert
        Assert.Empty(testCase.Validations);
        result.AssertValue(expectedResult);
    }
    
    [Fact]
    public void Execute_WithExceptionThrownOperation_ShouldReturnException()
    {
        // Assign
        var exception = new CustomWithMessageException("Message");
        var container = TestSuiteFactory.CreateEmptyContextContainer(operation: (int _, int _) =>
        {
            throw exception;
        });
        
        var testCaseExecutor = new TestCaseExecutor();

        var testCase = TestSuiteFactory.CreateAndAddTestCase(container, [1, 2], 3);
        var sw = new Stopwatch();
        
        // Act
        var result = testCaseExecutor.Execute(testCase, sw);

        // Assert
        Assert.Empty(testCase.Validations);
        result.AssertException(exception);
    }
    
    [Fact]
    public void Execute_InvalidInput_ShouldReturnExceptionWithValidation()
    {
        // Assign
        var exception = new CustomWithMessageException("Message");
        var input = new[] { 1, 2, 3 }.Cast<object?>().ToArray();
        
        var container = TestSuiteFactory.CreateEmptyContextContainer(operation: (int x, int y) => x + y);
        
        var testCaseExecutor = new TestCaseExecutor();

        var testCase = TestSuiteFactory.CreateAndAddTestCase(container, input, 6);
        var sw = new Stopwatch();
        
        // Act
        var result = testCaseExecutor.Execute(testCase, sw);

        // Assert
        Assert.Single(testCase.Validations);
        result.AssertException(new System.Reflection.TargetParameterCountException("Parameter count mismatch."));
    }
    
    [Fact]
    public void Execute_EmptyOperation_ShouldReturnNullWithValidation()
    {
        // Assign
        var input = new[] { 1, 2, 3 }.Cast<object?>().ToArray();
        
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        
        var testCaseExecutor = new TestCaseExecutor();

        var testCase = TestSuiteFactory.CreateAndAddTestCase(container, input, 6);
        var sw = new Stopwatch();
        
        // Act
        var result = testCaseExecutor.Execute(testCase, sw);

        // Assert
        Assert.NotNull(result);
        Assert.Single(testCase.Validations);
        result.AssertNull();
    }
}