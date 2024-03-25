using System.Diagnostics;
using Moq;
using SimpleFluentTester.TestSuite.Case;
using SimpleFluentTester.TestSuite.ComparedObject;
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
        
        var comparedObjectFactory = new ComparedObjectFactory();
        var comparedObjectFactoryMock = new Mock<IComparedObjectFactory>();
        comparedObjectFactoryMock
            .Setup(x => x.Wrap(It.Is<int>(y => y == expectedResult)))
            .Returns(comparedObjectFactory.Wrap(expectedResult));
        var testCaseExecutor = new TestCaseExecutor(container.Context, comparedObjectFactoryMock.Object);

        var testCase = TestSuiteFactory.CreateTestCase(input, expectedResult);
        var sw = new Stopwatch();
        
        // Act
        var result = testCaseExecutor.Execute(testCase, sw);

        // Assert
        comparedObjectFactoryMock
            .Verify(x => x.Wrap(It.Is<int>(y => y == expectedResult)), Times.Once);
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
        
        var comparedObjectFactory = new ComparedObjectFactory();
        var comparedObjectFactoryMock = new Mock<IComparedObjectFactory>();
        comparedObjectFactoryMock
            .Setup(x => x.Wrap(It.Is<CustomWithMessageException>(y => y == exception)))
            .Returns(comparedObjectFactory.Wrap(exception));
        var testCaseExecutor = new TestCaseExecutor(container.Context, comparedObjectFactoryMock.Object);

        var testCase = TestSuiteFactory.CreateTestCase([1, 2], 3);
        var sw = new Stopwatch();
        
        // Act
        var result = testCaseExecutor.Execute(testCase, sw);

        // Assert
        comparedObjectFactoryMock
            .Verify(x => x.Wrap(It.Is<CustomWithMessageException>(y => y == exception)), Times.Once);
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
        
        var comparedObjectFactory = new ComparedObjectFactory();
        var comparedObjectFactoryMock = new Mock<IComparedObjectFactory>();
        comparedObjectFactoryMock
            .Setup(x => x.Wrap(It.IsAny<Exception>()))
            .Returns(comparedObjectFactory.Wrap(exception));
        var testCaseExecutor = new TestCaseExecutor(container.Context, comparedObjectFactoryMock.Object);

        var testCase = TestSuiteFactory.CreateTestCase(input, 6);
        var sw = new Stopwatch();
        
        // Act
        var result = testCaseExecutor.Execute(testCase, sw);

        // Assert
        comparedObjectFactoryMock
            .Verify(x => x.Wrap(It.IsAny<Exception>()), Times.Once);
        Assert.Single(testCase.Validations);
        result.AssertException(exception);
    }
    
    [Fact]
    public void Execute_EmptyOperation_ShouldReturnNullWithValidation()
    {
        // Assign
        var input = new[] { 1, 2, 3 }.Cast<object?>().ToArray();
        
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        
        var comparedObjectFactory = new ComparedObjectFactory();
        var testCaseExecutor = new TestCaseExecutor(container.Context, comparedObjectFactory);

        var testCase = TestSuiteFactory.CreateTestCase(input, 6);
        var sw = new Stopwatch();
        
        // Act
        var result = testCaseExecutor.Execute(testCase, sw);

        // Assert
        Assert.NotNull(result);
        Assert.Single(testCase.Validations);
        result.AssertNull();
    }
}