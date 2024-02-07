using SimpleFluentTester;
using SimpleFluentTester.Examples;

// Setup test suite with default reporter (default output format).
// Then add few test cases, run them and print report.
TestSuite.Setup()
    .UseOperation<int>(CustomMethods.Adder) // UseOperation generic type should be the same as used delegate's return type.  
    .AddTestCase(2, 1, 1) // Number and type of input parameters should be the same as delegate's parameters, otherwise exception will be thrown.
    .AddTestCase(-2, -1, -1)
    .AddTestCase(-3, -1, -1)
    .Run() // Could be used as .Run(1, 2) to run some specific test cases.
    .Report(); // Prints the test execution result using default reporter.

// Setup test suite with custom reporter CustomReporterFactory.
// Then add few test cases, run them and print report.
TestSuite.Custom
    .WithCustomReporterFactory<CustomReporterFactory>() 
    .Setup()
    .UseOperation<int>(CustomMethods.Adder) // UseOperation generic type should be the same as used delegate's return type. 
    .AddTestCase(2, 1, 1)  // Number and type of input parameters should be the same as delegate's parameters, otherwise exception will be thrown.
    .AddTestCase(-2, -1, -1)
    .AddTestCase(-3, -1, -1)
    .Run() // Could be used as .Run(1, 2) to run some specific test cases.
    .Report();  // Prints the test execution result using custom CustomReporterFactory reporter.