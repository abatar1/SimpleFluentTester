using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class WithInputTests
{
    [Fact]
    public void WithInput_ParametersNumberMoreThanExpected_ShouldBeInvalid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .ExpectReturn(2).WithInput(1, 1, 1)
            .Run();
        
        // Assert
        var message = "Invalid inputs number, should have 2 parameters, but had 3: [1, 1, 1]";
        reporter.AssertTestCaseExists(1).AssertNonValid(ValidationSubject.Inputs, message);
    }
    
    [Fact]
    public void WithInput_ParametersNumberLessThanExpected_ShouldBeInvalid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .ExpectReturn(2).WithInput(1)
            .Run();
        
        // Assert
        var message = "Invalid inputs number, should have 2 parameters, but had 1: [1]";
        reporter.AssertTestCaseExists(1).AssertNonValid(ValidationSubject.Inputs, message);
    }
    
    [Fact]
    public void WithInput_ParametersWrongType_ShouldBeInvalid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .ExpectReturn(2).WithInput(1, "test")
            .Run();
        
        // Assert
        var message = "Passed parameters and expected operation parameters are not equal.";
        reporter.AssertTestCaseExists(1).AssertNonValid(ValidationSubject.Inputs, message);
    }
    
    [Fact]
    public void WithInput_InputNumberAndTypesCompatibleWithOperation_ShouldBeValid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .ExpectReturn(2).WithInput(1, 1)
            .Run();
        
        // Assert
        reporter.AssertValid();
    }
}