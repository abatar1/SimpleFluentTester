using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestCase.Pipeline;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.TestSuite.Parameter;
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
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        var testCase = container.DefineTestCaseWithExpectedValue(input, expectedResult);
        
        // Act
        var executedTestCase = testCase.Execute();

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.NotEmpty(testCase.Validations);
        executedTestCase.Clauses.AssertSingleValue(expectedResult);
    }
    
    [Fact]
    public void Execute_ValidExecutionWithArray_ShouldReturnValue()
    {
        // Assign
        int[] expectedResult = [1, 2, 3, 4];
        object?[] input = [new[] { 1, 2 }, new[] { 3, 4 }];
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int[] x, int[] y) => x.Concat(y).ToArray());
        var testCase = container.DefineTestCaseWithExpectedValue(input, expectedResult);
        
        // Act
        var executedTestCase = testCase.Execute();

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.NotEmpty(testCase.Validations);
        executedTestCase.Clauses.AssertSingleValue(expectedResult);
    }
    
    [Fact]
    public void Execute_ValidExecutionWithArrayResultAndValues_ShouldReturnValue()
    {
        // Assign
        int[] expectedResult = [1, 2];
        object?[] input = [1, 2];
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => new[] { x, y });
        var testCase = container.DefineTestCaseWithExpectedValue(input, expectedResult);
        
        // Act
        var executedTestCase = testCase.Execute();

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.NotEmpty(testCase.Validations);
        executedTestCase.Clauses.AssertSingleValue(expectedResult);
    }
    
    [Fact]
    public void Execute_ValidExecutionWithParameter_ShouldReturnValue()
    {
        // Assign
        int expectedResult = 5;
        var expectedParameter = new[] {3, 5};
        Delegate operation = (int[] seq1, int[] seq2) =>
        {
            for (var i = 0; i < seq1.Length; i++)
                seq1[i] += seq2[i];
            return seq1[0] + seq2[0];
        };
        var container = TestSuiteContextContainer.Default();
        container.WithOperation(operation);
        var expectedReturnClause = TestClauseFactory.DefineFromValue(expectedResult);
        var parameter = ExpectParameterFactory.Create(container, 0);
        var expectedParameterClause = TestClauseFactory.DefineFromParameter(expectedParameter, parameter);
        var clauses = expectedReturnClause.Concat(expectedParameterClause).ToList();
        var testCase = container.DefineTestCase([new[] {1, 2}, new[] {2, 3}], clauses);
        
        // Act
        var executedTestCase = testCase.Execute();

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.NotEmpty(testCase.Validations);
        
        var executedClauses = executedTestCase.Clauses.Cast<ExecutedTestClause>().ToList();

        var returnClause = executedClauses
            .FirstOrDefault(clause => clause.Expected is ValueObject);
        Assert.NotNull(returnClause);
        returnClause.Result.AssertSingleValue(expectedResult);
        
        var parameterClause = executedClauses
            .FirstOrDefault(clause => clause.Expected is ParameterObject);
        Assert.NotNull(parameterClause);
        parameterClause.Result.AssertSingleValue(expectedParameter);
    }
    
    [Fact]
    public void Execute_WithExceptionThrownOperation_ShouldReturnException()
    {
        // Assign
        var exception = new CustomWithMessageException("Message");
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int _, int _) => { throw exception; });
        var testCase = container.DefineTestCaseWithExpectedValue([1, 2], 3);
        
        // Act
        var executedTestCase = testCase.Execute();

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.NotEmpty(testCase.Validations);
        executedTestCase.Clauses.AssertSingleException(exception);
    }
    
    [Fact]
    public void Execute_InvalidInput_ShouldReturnExceptionWithValidation()
    {
        // Assign
        var input = new[] { 1, 2, 3 }.Cast<object?>().ToArray();
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        var testCase = container.DefineTestCaseWithExpectedValue(input, 6);
        
        // Act
        var executedTestCase = testCase.Execute();

        // Assert
        Assert.NotNull(executedTestCase);
        Assert.NotEmpty(testCase.Validations);
        executedTestCase.Clauses.AssertSingleException(new System.Reflection.TargetParameterCountException("Parameter count mismatch."));
    }
    
    [Fact]
    public void Execute_EmptyOperation_ShouldThrowException()
    {
        // Assign
        var input = new[] { 1, 2, 3 }.Cast<object?>().ToArray();
        var container = TestSuiteContextContainer.Default();
        var testCase = container.DefineTestCaseWithExpectedValue(input, 6);
        
        // Act
        var func = () => testCase.Execute();

        // Assert
        Assert.Throws<InvalidContextException>(func);
    }
}