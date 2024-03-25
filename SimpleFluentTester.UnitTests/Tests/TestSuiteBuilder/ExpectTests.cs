using System.Reflection;
using Moq;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.TestObjects;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public class ExpectTests
{
    [Fact]
    public void Expect_SetValidOperationWithValidExpectedValue_ShouldBePassed()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed(2, [1, 1]);
    }
    
    [Fact]
    public void Expect_OperationWithNullableParameter_ShouldBePassed()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation((int? a, int b) => a + b);
            
        // Act    
        var reporter = builder
            .Expect(null).WithInput(null, 1)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed<int?>(null, [null, 1]);
    }
    
    [Fact]
    public void Expect_OperationWithNullableParameterButActualValue_ShouldNotThrow()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation((int? a, int? b) => a + b);
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(3).WithInput(1, 1)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed(2, [1, 1]);
        reporter.AssertTestCaseExists(2).AssertNotPassed(3, [1, 1]);
    }
    
    [Fact]
    public void Expect_ParametersValidTypesAndCount_ValidReturn()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(2).WithInput(2, 1)
            .Expect(3).WithInput(2, 1)
            .Run(1, 2);

        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed(2, [1, 1]);
        reporter.AssertTestCaseExists(2).AssertNotPassed(2, [2, 1]);
        reporter.AssertTestCaseExists(3).AssertSkippedTestResult(3, [2, 1]);
    }

    [Fact]
    public void Expect_TestingOperationThrowsExceptionAndExpectNumber_TestCaseHasException()
    {
        // Assign
        Func<int, int, int> comparer = (_, _) => throw new CustomException();
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation(comparer);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertNotPassedWithException(2, [1, 1], typeof(CustomException));
    }
    
    [Fact]
    public void Expect_OperationNotSetWithCorrectAssembly_AdderWithAttributeShouldBeSelected()
    {
        // Assign
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock
            .Setup(x => x.Get())
            .Returns(Assembly.GetAssembly(typeof(ExpectTests)));
        var container = TestSuiteFactory.CreateEmptyContextContainer(entryAssemblyProviderMock.Object);
        var builder = new TestSuite.TestSuiteBuilder(container);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertPassed(2, [1, 1]);
    }
    
    [Fact]
    public void Expect_OperationNotSetWithNotCorrectAssembly_AdderWithAttributeShouldNotBeSelected()
    {
        // Assign
        var assemblyMock = new Mock<Assembly>();
        assemblyMock
            .Setup(x => x.GetTypes())
            .Returns(Array.Empty<Type>());
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock
            .Setup(x => x.Get())
            .Returns(assemblyMock.Object);
        var container = TestSuiteFactory.CreateEmptyContextContainer(entryAssemblyProviderMock.Object);
        var builder = new TestSuite.TestSuiteBuilder(container);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Operation, "Operation not specified");
    }
    
    [Fact]
    public void Expect_WithCustomEquatableObject_ValidReturn()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .WithComparer<EquatableTestObject>((x, y) => x?.Value == y?.Value)
            .UseOperation((EquatableTestObject a, EquatableTestObject b) => new EquatableTestObject(a.Value + b.Value));
        
        // Act
        var reporter = builder
            .Expect(new EquatableTestObject(2)).WithInput(new EquatableTestObject(1), new EquatableTestObject(1))
            .Run();
        
        // Assert
        reporter
            .AssertTestCaseExists(1)
            .AssertPassed(new EquatableTestObject(2), [new EquatableTestObject(1), new EquatableTestObject(1)]);
    }
    
    [Fact]
    public void Expect_WithCustomEquatableObjectAndInvalidComparer_FailedReturn()
    {
        // Assign
        var exceptionMessage = "Exception";
        var exception = new CustomWithMessageException(exceptionMessage);
        var builder = TestSuite.TestSuite.Sequential
            .WithComparer<EquatableTestObject>((x, y) => throw exception)
            .UseOperation((EquatableTestObject a, EquatableTestObject b) => new EquatableTestObject(a.Value + b.Value));
        
        // Act
        var reporter = builder
            .Expect(new EquatableTestObject(2)).WithInput(new EquatableTestObject(1), new EquatableTestObject(1))
            .Run();
        
        // Assert
        reporter
            .AssertTestCaseExists(1)
            .AssertFailed(exception, exceptionMessage);
    }
    
    [Fact]
    public void Expect_NoReturnTypeSpecifiedWithInvalidExpectedType_ShouldBeInvalid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect("123").WithInput(1, 1)
            .Run();
        
        // Assert
        var message = "Operation return type is not the same as used generic type.";
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Operation, message);
    }
    
    [Fact]
    public void Expect_OperationWithoutNullableParameterButNullExpected_ShouldBeInvalid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect(null).WithInput(1, 1)
            .Run();
        
        // Assert
        var message = "Operation return type is not the same as used generic type.";
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Operation, message);
    }
    
    [Fact]
    public void Expect_EqualOperationAndExpectType_ShouldBeValid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).Validation.AssertValid();
    }
}