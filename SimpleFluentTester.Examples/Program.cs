using SimpleFluentTester;
using SimpleFluentTester.Examples;

// 1 Example.
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

// 2 Example.
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
    
// 3 Example.
// This example shows that UseOperation could be skipped.
// Instead, TestSuiteDelegateAttribute could be used on target method.
TestSuite
    .WithExpectedReturnType<int>()
    .Expect(2).WithInput(1, 1)
    .Expect(-2).WithInput(-1, -1)
    .Expect(-3).WithInput(-1, -1)
    .Run()
    .Report();

// 4 Example.
// This example demonstrates how custom types can be used with the TestSuite.
// To achieve this, define a comparer function using WithExpectedReturnType().
TestSuite
    .WithExpectedReturnType<CustomValue>((x, y) => x.Value == y.Value)
    .UseOperation(CustomMethods.CustomAdder) 
    .Expect(CustomValue.FromInt(2)).WithInput(CustomValue.FromInt(1), CustomValue.FromInt(1))
    .Expect(CustomValue.FromInt(-2)).WithInput(CustomValue.FromInt(-1), CustomValue.FromInt(-1))
    .Expect(CustomValue.FromInt(-3)).WithInput(CustomValue.FromInt(-1), CustomValue.FromInt(-1))
    .Run()
    .Report();
    
    