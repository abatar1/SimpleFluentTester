using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public class WithInputTests
{
    [Fact]
    public void WithInput_ParametersNumberMoreThanExpected_ShouldBeInvalid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1, 1)
            .Run();
        
        // Assert
        var message = "Invalid inputs number, should be 2, but was 3, inputs 1, 1, 1";
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Inputs, message);
    }
    
    [Fact]
    public void WithInput_ParametersNumberLessThanExpected_ShouldBeInvalid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1)
            .Run();
        
        // Assert
        var message = "Invalid inputs number, should be 2, but was 1, inputs 1";
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Inputs, message);
    }
    
    [Fact]
    public void WithInput_ParametersWrongType_ShouldBeInvalid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, "test")
            .Run();
        
        // Assert
        var message = "Passed parameters and expected operation parameters are not equal";
        reporter.AssertTestCaseExists(1).Validation.AssertInvalid(ValidationSubject.Inputs, message);
    }
    
    [Fact]
    public void WithInput_InputNumberAndTypesCompatibleWithOperation_ShouldBeValid()
    {
        // Assign
        var builder = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        reporter.TestSuiteResult.Validation.AssertValid();
    }
}