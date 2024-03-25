using SimpleFluentTester.UnitTests.Extensions;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public class RunTests
{
    [Fact]
    public void Run_InvalidIterationNumber_ShouldBeInvalid()
    {
        // Assign
        var builder1 = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
        var builder2 = TestSuite.TestSuite.Sequential
            .UseAdderOperation();
        
        // Act
        var reporter1 = builder1
            .Expect(2).WithInput(1, 1)
            .Run(2);
        var reporter2 = builder2
            .Expect(2).WithInput(1, 1)
            .Run(1, 2);
        
        // Assert
        var message = "Invalid test case numbers were given as input";
        reporter1.TestSuiteResult.Validation.AssertInvalid(ValidationSubject.TestNumbers, message);
        reporter2.TestSuiteResult.Validation.AssertInvalid(ValidationSubject.TestNumbers, message);
    }
}