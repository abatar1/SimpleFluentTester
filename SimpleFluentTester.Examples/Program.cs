using SimpleFluentTester.Examples;
using SimpleFluentTester.TestSuite;

// Uncomment the following line to allow only specific test cases or ignore specific tests.
// TestSuite.Allow(7);
// TestSuite.Ignore(1, 2);

// Example 1.
// Setup test suite with a default reporter (default output format).
// Then add a few test cases, run them and print a report.
// Adder has a signature int (int x, int y).
TestSuite.Sequential
    .WithDisplayName("Example 1, 2 not passed")
    .UseOperation(CustomMethods.Adder)  
    .ExpectReturn(2).WithInput(1, 1) // The number and type of input parameters should be the same as delegate's parameters, otherwise exception will be thrown.
    .ExpectReturn(-3).WithInput(-1, -1)
    .Run() // Could be used as .Run(1, 2) to run some specific test cases.
    .Report(); // Prints the test execution result using default reporter.

// Example 2.
// Setup test suite with custom reporter CustomReporterFactory.
// Then add a few test cases, run them and print a report.
// Adder has a signature int (int x, int y).
TestSuite.Sequential
    .WithDisplayName("Example 2, all should pass")
    .UseOperation(CustomMethods.Adder) 
    .ExpectReturn(2).WithInput(1, 1)
    .ExpectReturn(-3).WithInput(-1, -1)
    .Run()
    .Report((builder, _) =>
    {
        builder
            .WithReportBuilder<CustomTestSuiteReportBuilder>()
            .WithPrintablePredicate((testClause, _) => testClause.Assert.Status == AssertStatus.Passed);
    });
    
// Example 3.
// This example shows that UseOperation could be skipped.
// Instead, TestSuiteDelegateAttribute could be used on the target method.
TestSuite.Sequential
    .WithDisplayName("Example 3, 2 not passed")
    .ExpectReturn(2).WithInput(1, 1)
    .ExpectReturn(-3).WithInput(-1, -1)
    .Run()
    .Report();

// Example 4.
// This example demonstrates how custom types can be used with the TestSuite.
// To achieve this, define a comparer function using WithExpectedReturnType().
// CustomValue has a signature CustomValue (CustomValue x, CustomValue y).
TestSuite.Sequential
    .WithDisplayName("Example 4, 2 not passed")
    .UseOperation(CustomMethods.CustomAdder) 
    .ExpectReturn(CustomValue.FromInt(2)).WithInput(CustomValue.FromInt(1), CustomValue.FromInt(1))
    .ExpectReturn(CustomValue.FromInt(-3)).WithInput(CustomValue.FromInt(-1), CustomValue.FromInt(-1))
    .WithComparer<CustomValue>((x, y) => x?.Value == y?.Value)
    .Run()
    .Report();

// Example 5.
// Invalid inputs, test case number and expected value to demonstrate validation output.
// Adder has a signature int (int x, int y).
TestSuite.Sequential
    .WithDisplayName("Example 5, all not passed")
    .UseOperation(CustomMethods.Adder) 
    .WithComparer<int>((x, y) => x == y)
    .ExpectReturn(null).WithInput(1, 1)
    .ExpectReturn([1, 2]).WithInput(1, 1)
    .ExpectReturn("test").WithInput(1, 1)
    .ExpectReturn(-3).WithInput(-1, -1, -1)
    .ExpectReturn("-3").WithInput("test", -1)
    .Run()
    .Report();

// Example 6.
// Broken adder that throws an exception.
// BrokenAdder has a signature int (int x, int y).
TestSuite.Sequential
    .WithDisplayName("Example 6, 2 not passed")
    .UseOperation(CustomMethods.BrokenAdder)
    .ExpectException<AdderException>(CustomMethods.BrokenAdderMessage).WithInput(1, 2)
    .ExpectException<Exception>(CustomMethods.BrokenAdderMessage).WithInput(1, 2)
    .Run()
    .Report();

// Example 7.
// Demonstrates the case with positional argument value changing and void result.
// VoidPositionalSeqAdder has a signature void (int[] seq1, int[] seq2).
TestSuite.Sequential
    .WithDisplayName("Example 7, 3 not passed")
    .UseOperation(CustomMethods.VoidPositionalSeqAdder)
    .ExpectParameter(0).ToBe([2, 4]).WithInput([0, 1], [2, 3])
    .ExpectParameter("seq1").ToBe([2, 4]).WithInput([0, 1], [2, 3])
    .ExpectParameter(0).ToBe([2, 3]).WithInput([0, 1], [2, 3])
    .Run()
    .Report();

// Example 8.
// Demonstrates the case with positional argument value changing and int result.
// PositionalSeqAdder has a signature int (int[] seq1, int[] seq2).
TestSuite.Sequential
    .WithDisplayName("Example 8, Test 2 Clause 2 not passed")
    .UseOperation(CustomMethods.PositionalSeqAdder)
    .ExpectParameter(0).ToBe([4, 3, 5]).And.ExpectReturn(7).WithInput([1, 2, 3], [3, 1, 2])
    .ExpectParameter("seq1").ToBe([1, 3, 5]).And.ExpectReturn(2).WithInput([1, 2, 3], [0, 1, 2])
    .Run()
    .Report();

    