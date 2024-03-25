using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.TestObjects;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public class UseOperationTests
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
        reporter.TestSuiteResult.Validation.AssertValid();
    }
    
    [Fact]
    public void UseOperation_WithCustomObjectAndComparer_ValidReturn()
    {
        // Assign
        var comparer = (NotEquatableTestObject? a, NotEquatableTestObject? b) => a?.Value == b?.Value;
        var setup = TestSuite.TestSuite.Sequential
            .WithComparer(comparer);
        
        // Act
        var reporter = setup
            .UseOperation((NotEquatableTestObject a, NotEquatableTestObject b) => new NotEquatableTestObject(a.Value + b.Value))
            .Expect(new NotEquatableTestObject(2)).WithInput([new NotEquatableTestObject(1), new NotEquatableTestObject(1)])
            .Run();

        // Assert
        reporter
            .AssertTestCaseExists(1)
            .AssertPassed(new NotEquatableTestObject(2), [new NotEquatableTestObject(1), new NotEquatableTestObject(1)], comparer);
    }
    
    [Fact]
    public void UseOperation_NoReturn_ShouldBeInvalid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .WithComparer<int>((x, y) => x == y);
        
        // Act
        var reporter = setup
            .Expect(2).WithInput(1, 1)
            .UseOperation((int _, int _) => { })
            .Run();

        // Assert
        var message = "Operation must have return type to be testable";
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Operation, message);
    }
    
    [Fact]
    public void UseOperation_NoOperationSet_ShouldBeInvalid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .WithComparer<int>((x, y) => x == y);
        
        // Act
        var reporter = setup
            .Run();

        // Assert
        var message = "You should specify an operation first with an TestSuiteDelegateAttribute attribute or using UseOperation method.";
        reporter.TestSuiteResult.Validation.AssertInvalid(ValidationSubject.Operation, message);
    }
    
    [Fact]
    public void UseOperation_InvalidDelegateReturnType_ShouldBeInvalid()
    {
        // Assign
        var container = TestSuiteHelper.CreateEmptyContextContainer();
        var builder = new TestSuite.TestSuiteBuilder(container);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .UseOperation((int _, int _) => "test")
            .Run();
        
        // Assert
        var message = "Operation return type is not the same as used generic type.";
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Operation, message);
    }
}