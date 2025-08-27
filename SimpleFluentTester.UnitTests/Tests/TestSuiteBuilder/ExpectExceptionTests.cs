using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class ExpectExceptionTests
{
    [Fact]
    public void ExpectException_CustomExceptionWithoutMessage_ShouldBeValid()
    {
        // Assign
        Delegate operation = new Func<int, int, int>((int x, int y) =>
        {
            if (x + y == 2)
                throw new CustomException();
            return x + y;
        }); 
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation(operation);
            
        // Act    
        var reporter = builder
            .ExpectException<CustomException>().WithInput(1, 1)
            .ExpectReturn(3).WithInput(1, 2)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed<object>(new CustomException(), [1, 1]);
    }
    
    [Fact]
    public void ExpectException_CustomExceptionWithoutStringCtorWithMessage_ShouldBeInvalid()
    {
        // Assign
        Delegate operation = new Func<int, int, int>((int x, int y) =>
        {
            if (x + y == 2)
                throw new CustomException();
            return x + y;
        }); 
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation(operation);
            
        // Act    
        var reporter = builder
            .ExpectException<CustomException>("Test").WithInput(1, 1)
            .ExpectReturn(3).WithInput(1, 2)
            .Run();
        
        // Assert
        var message = $"{typeof(CustomException).FullName} do not have public .ctor() with string parameter";
        reporter.AssertTestCaseExists(1).AssertNonValid(ValidationSubject.Expect, message);
    }
    
    [Fact]
    public void ExpectException_CustomExceptionWithStringCtorWithMessage_ShouldBeValid()
    {
        // Assign
        var exceptionMessage = "Test";
        Delegate operation = new Func<int, int, int>((int x, int y) =>
        {
            if (x + y == 2)
                throw new CustomWithMessageException(exceptionMessage);
            return x + y;
        }); 
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation(operation);
            
        // Act    
        var reporter = builder
            .ExpectException<CustomWithMessageException>(exceptionMessage).WithInput(1, 1)
            .ExpectReturn(3).WithInput(1, 2)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed<object>(new CustomWithMessageException(exceptionMessage), [1, 1]);
    }

    
}