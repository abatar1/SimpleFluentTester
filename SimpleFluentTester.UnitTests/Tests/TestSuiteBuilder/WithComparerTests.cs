using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class WithComparerTests
{
    [Fact]
    public void WithComparer_UseCustomObjectWithoutComparer_ShouldBeInvalid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .ExpectReturn(new NotEquatableTestObject(1)).WithInput(new NotEquatableTestObject(1))
            .UseOperation((NotEquatableTestObject x) => x);
        
        // Act
        var reporter = setup
            .Run();

        // Assert
        var message = $"{typeof(NotEquatableTestObject).FullName} type should be assignable from {typeof(IEquatable<>).Name} or comparer should be defined";
        reporter.AssertNonValid(ValidationSubject.Comparer, message);
    }
    
    [Fact]
    public void WithComparer_ExpectStringWithIntComparer_ShouldBeInvalid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .ExpectReturn("test").WithInput("test")
            .UseOperation((int x) => x);
        
        // Act
        var reporter = setup
            .WithComparer<int>((x, y) => x == y)
            .Run();

        // Assert
        var message = "Test case type was System.String, but comparer type is System.Int32";
        reporter.AssertNonValid(ValidationSubject.Comparer, message);
    }
    
    [Fact]
    public void WithComparer_ExpectIntWithIntComparer_ShouldBeValid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .UseOperation((int x) => x)
            .ExpectReturn(1).WithInput(1);
        
        // Act
        var reporter = setup
            .WithComparer<int>((x, y) => x == y)
            .Run();

        // Assert
        reporter.AssertValid();
    }
    
    [Fact]
    public void WithComparer_ExpectIntWithStringComparer_ShouldBeInvalid()
    {
        // Assign
        var setup = TestSuite.TestSuite.Sequential
            .UseOperation((int x) => x)
            .ExpectReturn(1).WithInput(1);
        
        // Act
        var reporter = setup
            .WithComparer<string>((x, y) => x == y)
            .Run();

        // Assert
        var message = "Test case type was System.Int32, but comparer type is System.String";
        reporter.AssertNonValid(ValidationSubject.Comparer, message);
    }
}