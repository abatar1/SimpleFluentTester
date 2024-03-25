using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.TestObjects;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public class ExpectExceptionTests
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
            .Expect(3).WithInput(1, 2)
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
            .Expect(3).WithInput(1, 2)
            .Run();
        
        // Assert
        var message = "SimpleFluentTester.UnitTests.Helpers.CustomException do not have public ctor with string parameter";
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Expect, message);
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
            .Expect(3).WithInput(1, 2)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed<object>(new CustomWithMessageException(exceptionMessage), [1, 1]);
    }

    
}