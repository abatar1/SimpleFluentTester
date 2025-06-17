using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class UseOperationTests
{
    [Fact]
    public void UseOperation_ValidReturnType_ShouldBeValid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .WithComparer<int>((x, y) => x == y);
        
        // Act
        var reporter = setup
            .UseAdderOperation()
            .Run();

        // Assert
        reporter.AssertValid();
    }
    
    [Fact]
    public void UseOperation_WithCustomObjectAndComparer_ValidReturn()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .WithComparer<NotEquatableTestObject?>((a, b) => a?.Value == b?.Value);
        
        // Act
        var reporter = setup
            .UseOperation((NotEquatableTestObject a, NotEquatableTestObject b) => new NotEquatableTestObject(a.Value + b.Value))
            .ExpectResult(new NotEquatableTestObject(2)).WithInput([new NotEquatableTestObject(1), new NotEquatableTestObject(1)])
            .Run();

        // Assert
        reporter
            .AssertTestCaseExists(1)
            .AssertPassed(new NotEquatableTestObject(2), [new NotEquatableTestObject(1), new NotEquatableTestObject(1)], (a, b) => a?.Value == b?.Value);
    }
    
    [Fact]
    public void UseOperation_NoReturn_ShouldBeInvalid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .WithComparer<int>((x, y) => x == y);
        
        // Act
        var reporter = setup
            .ExpectResult(2).WithInput(1, 1)
            .UseOperation((int _, int _) => { })
            .Run();

        // Assert
        var message = "Operation must have return type to be testable";
        reporter.AssertTestCaseExists(1).AssertNonValid(ValidationSubject.Operation, message);
    }
    
    [Fact]
    public void UseOperation_NoOperationSet_ShouldBeInvalid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .WithComparer<int>((x, y) => x == y);
        
        // Act
        Action func = () => setup
            .Run();

        // Assert
        var message = "You should specify an operation first with an TestSuiteDelegateAttribute attribute or using UseOperation method.";
        TestHelpers.AssertWithMessage<InvalidContextException>(func, message);
    }
    
    [Fact]
    public void UseOperation_InvalidDelegateReturnType_ShouldBeInvalid()
    {
        // Assign
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var builder = new TestSuite.TestSuiteBuilder(container);
        
        // Act
        var reporter = builder
            .ExpectResult(2).WithInput(1, 1)
            .UseOperation((int _, int _) => "test")
            .Run();
        
        // Assert
        var message = "Operation return type is not the same as used generic type.";
        reporter.AssertTestCaseExists(1).AssertNonValid(ValidationSubject.Operation, message);
    }
}