using Microsoft.Extensions.Logging;
using SimpleFluentTester.Examples;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Case;

//TestSuite.Allow(7);

// Example 1.
// Setup test suite with a default reporter (default output format).
// Then add a few test cases, run them and print a report.
// Adder has a signature int (int x, int y).
TestSuite.Sequential
    .WithDisplayName("Example 1, 2 is not valid")
    .UseOperation(CustomMethods.Adder)  
    .ExpectResult(2).WithInput(1, 1) // The number and type of input parameters should be the same as delegate's parameters, otherwise exception will be thrown.
    .ExpectResult(-3).WithInput(-1, -1)
    .Run() // Could be used as .Run(1, 2) to run some specific test cases.
    .Report(); // Prints the test execution result using default reporter.

// Example 2.
// Setup test suite with custom reporter CustomReporterFactory.
// Then add a few test cases, run them and print a report.
// Adder has a signature int (int x, int y).
TestSuite.Sequential
    .WithDisplayName("Example 2, all should be valid")
    .UseOperation(CustomMethods.Adder) 
    .ExpectResult(2).WithInput(1, 1)
    .ExpectResult(-3).WithInput(-1, -1)
    .Run()
    .Report((builder, _) =>
    {
        builder
            .WithReportBuilder(() => new CustomTestSuiteReportBuilder())
            .WithPrintablePredicate(testCase => testCase.Assert.Status == AssertStatus.NotPassed)
            .WithLoggingBuilder(x => x.AddSimpleConsole());
    });
    
// Example 3.
// This example shows that UseOperation could be skipped.
// Instead, TestSuiteDelegateAttribute could be used on the target method.
TestSuite.Sequential
    .WithDisplayName("Example 3, 2 is invalid")
    .ExpectResult(2).WithInput(1, 1)
    .ExpectResult(-3).WithInput(-1, -1)
    .Run()
    .Report();

// Example 4.
// This example demonstrates how custom types can be used with the TestSuite.
// To achieve this, define a comparer function using WithExpectedReturnType().
// CustomValue has a signature CustomValue (CustomValue x, CustomValue y).
TestSuite.Sequential
    .WithDisplayName("Example 4, 2 is invalid")
    .UseOperation(CustomMethods.CustomAdder) 
    .ExpectResult(CustomValue.FromInt(2)).WithInput(CustomValue.FromInt(1), CustomValue.FromInt(1))
    .ExpectResult(CustomValue.FromInt(-3)).WithInput(CustomValue.FromInt(-1), CustomValue.FromInt(-1))
    .WithComparer<CustomValue>((x, y) => x?.Value == y?.Value)
    .Run()
    .Report();

// Example 5.
// Invalid inputs, test case number and expected value to demonstrate validation output.
// Adder has a signature int (int x, int y).
TestSuite.Sequential
    .WithDisplayName("Example 5, all invalid")
    .UseOperation(CustomMethods.Adder) 
    .WithComparer<int>((x, y) => x == y)
    .ExpectResult(null).WithInput(1, 1)
    .ExpectResult("test").WithInput(1, 1)
    .ExpectResult(-3).WithInput(-1, -1, -1)
    .ExpectResult("-3").WithInput("test", -1)
    .Run()
    .Report();

// Example 6.
// Broken adder that throws an exception.
// BrokenAdder has a signature int (int x, int y).
TestSuite.Sequential
    .WithDisplayName("Example 6, 2 is invalid")
    .UseOperation(CustomMethods.BrokenAdder)
    .ExpectException<AdderException>(CustomMethods.BrokenAdderMessage).WithInput(1, 2)
    .ExpectException<Exception>(CustomMethods.BrokenAdderMessage).WithInput(1, 2)
    .Run()
    .Report();

// Example 7.
// Demonstrates the case with positional argument value changing and void result.
// VoidPositionalSeqAdder has a signature void (int[] seq1, int[] seq2).
TestSuite.Sequential
    .WithDisplayName("Example 7, 3 is invalid")
    .UseOperation(CustomMethods.VoidPositionalSeqAdder)
    .ExpectParameter(0).ToBe(new[] {2, 4}).WithInput(new[] {0, 1}, new [] {2, 3})
    .ExpectParameter("seq1").ToBe(new[] {2, 4}).WithInput(new[] {0, 1}, new [] {2, 3})
    .ExpectParameter(0).ToBe(new[] {2, 3}).WithInput(new[] {0, 1}, new [] {2, 3})
    .Run()
    .Report();

// Example 8.
// Demonstrates the case with positional argument value changing and int result.
// PositionalSeqAdder has a signature int (int[] seq1, int[] seq2).
TestSuite.Sequential
    .WithDisplayName("Example 8, 3 is invalid")
    .UseOperation(CustomMethods.PositionalSeqAdder)
    .ExpectParameter(0).ToBe(new[] {1, 3, 5}).And.ExpectResult(2).WithInput(new[] {1, 2, 3}, new [] {0, 1, 2})
    .ExpectParameter("seq1").ToBe(new[] {1, 3, 5}).And.ExpectResult(2).WithInput(new[] {1, 2, 3}, new [] {0, 1, 2})
    .Run()
    .Report();

    