using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;

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
        
        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, input, expectedResult);
        
        // Act
        var executedTestCase = TestCaseExecutor.Execute(testCase);

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.Empty(testCase.Validations);
        executedTestCase.Result.AssertValue(expectedResult);
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

        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, [1, 2], 3);
        
        // Act
        var executedTestCase = TestCaseExecutor.Execute(testCase);

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.Empty(testCase.Validations);
        executedTestCase.Result.AssertException(exception);
    }
    
    [Fact]
    public void Execute_InvalidInput_ShouldReturnExceptionWithValidation()
    {
        // Assign
        var input = new[] { 1, 2, 3 }.Cast<object?>().ToArray();
        
        var container = TestSuiteFactory.CreateEmptyContextContainer(operation: (int x, int y) => x + y);

        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, input, 6);
        
        // Act
        var executedTestCase = TestCaseExecutor.Execute(testCase);

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.Single(testCase.Validations);
        executedTestCase.Result.AssertException(new System.Reflection.TargetParameterCountException("Parameter count mismatch."));
    }
    
    [Fact]
    public void Execute_EmptyOperation_ShouldThrowException()
    {
        // Assign
        var input = new[] { 1, 2, 3 }.Cast<object?>().ToArray();
        
        var container = TestSuiteFactory.CreateEmptyContextContainer();

        var testCase = TestSuiteFactory.CreateDeferredTestCase(container, input, 6);
        
        // Act
        var func = () => TestCaseExecutor.Execute(testCase);

        // Assert
        Assert.Throws<InvalidContextException>(func);
    }
}