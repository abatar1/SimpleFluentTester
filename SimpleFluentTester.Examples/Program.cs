using SimpleFluentTester;
using SimpleFluentTester.Examples;

// Setup test suite with default reporter (default output format).
// Then add few test cases, run them and print report.
TestSuite
    .WithExpectedReturnType<int>() // WithExpectedReturnType generic type should be the same as testable method return type. 
    .UseOperation(CustomMethods.Adder)  
    .Expect(2).WithInput(1, 1) // Number and type of input parameters should be the same as delegate's parameters, otherwise exception will be thrown.
    .Expect(-2).WithInput(-1, -1)
    .Expect(-3).WithInput(-1, -1)
    .Run() // Could be used as .Run(1, 2) to run some specific test cases.
    .Report(); // Prints the test execution result using default reporter.

// Setup test suite with custom reporter CustomReporterFactory.
// Then add few test cases, run them and print report.
TestSuite
    .WithExpectedReturnType<int>()
    .WithCustomReporterFactory<CustomReporterFactory>() 
    .UseOperation(CustomMethods.Adder) 
    .Expect(2).WithInput(1, 1)
    .Expect(-2).WithInput(-1, -1)
    .Expect(-3).WithInput(-1, -1)
    .Run()
    .Report();
    
// This example shows that UseOperation could be skipped.
// Instead, TestSuiteDelegateAttribute could be used on target method.
TestSuite
    .WithExpectedReturnType<int>()
    .Expect(2).WithInput(1, 1)
    .Expect(-2).WithInput(-1, -1)
    .Expect(-3).WithInput(-1, -1)
    .Run()
    .Report();