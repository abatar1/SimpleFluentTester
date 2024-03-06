using System.Reflection;
using Moq;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests;

public class TestRunBuilderTests
{
    [Fact]
    public void AddTestCase_NoReturnTypeSpecifiedWithValidExpectedType_ShouldBePassed()
    {
        // Arrange
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation(TestHelpers.Adder);
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        AssertPassedTestResult(1, reporter, 2, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_OperationWithNullableParameter_ShouldBePassed()
    {
        // Arrange
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation((int? a, int b) => a + b);
            
        // Act    
        var reporter = builder
            .Expect(null).WithInput(null, 1)
            .Run();
        
        // Assert
        AssertPassedTestResult(1, reporter, null, [null, 1]);
    }
    
    [Fact]
    public void AddTestCase_OperationWithNullableParameterButActualValue_ShouldNotThrow()
    {
        // Arrange
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation((int? a, int? b) => a + b);
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(3).WithInput(1, 1)
            .Run();
        
        // Assert
        AssertPassedTestResult(1, reporter, 2, [1, 1]);
        AssertNotPassedTestResult(2, reporter, 3, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_ParametersValidTypesAndCount_ValidReturn()
    {
        // Arrange
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation(TestHelpers.Adder);
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(2).WithInput(2, 1)
            .Expect(3).WithInput(2, 1)
            .Run(1, 2);

        // Assert
        AssertPassedTestResult(1, reporter, 2, [1, 1]);
        AssertNotPassedTestResult(2, reporter, 2, [2, 1]);
        AssertSkippedTestResult(3, reporter, 3, [2, 1]);
    }

    [Fact]
    public void AddTestCase_TestingOperationIsBroken_TestCaseHasException()
    {
        // Arrange
        Func<int, int, int> comparer = (_, _) => throw new CustomException();
        var builder = TestSuite.TestSuite.Sequential
            .UseOperation(comparer);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        var testCase = reporter.TestSuiteResult.TestCases
            .FirstOrDefault(x => x.Number == 1);
        Assert.NotNull(testCase);
        Assert.Equal(AssertStatus.NotPassedWithException, testCase.AssertStatus);
        Assert.Equal(ValidationStatus.Valid, testCase.ValidationStatus);
        Assert.NotNull(testCase.Assert);
        Assert.NotNull(testCase.Assert.Exception);
        Assert.IsType<CustomException>(testCase.Assert.Exception);
    }
    
    [Fact]
    public void AddTestCase_TestingOperationNotSet_AdderWithAttributeShouldBeSelected()
    {
        // Arrange
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock
            .Setup(x => x.Get())
            .Returns(Assembly.GetAssembly(typeof(TestRunBuilderTests)));
        var context = TestHelpers.CreateEmptyContext<int>(entryAssemblyProviderMock.Object);
        var builder = new TestSuiteBuilder<int>(context);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        AssertPassedTestResult(1, reporter, 2, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_WithCustomEquatableObject_ValidReturn()
    {
        // Arrange
        var builder = TestSuite.TestSuite.Sequential
            .WithComparer<EquatableTestObject>((x, y) => x?.Value == y?.Value)
            .UseOperation((EquatableTestObject a, EquatableTestObject b) => new EquatableTestObject(a.Value + b.Value));
        
        // Act
        var reporter = builder
            .Expect(new EquatableTestObject(2)).WithInput(new EquatableTestObject(1), new EquatableTestObject(1))
            .Run();
        
        // Assert
        AssertPassedTestResult(1, reporter, new EquatableTestObject(2), [new EquatableTestObject(1), new EquatableTestObject(1)]);
    }
    
    [Fact]
    public void UseOperation_WithCustomObjectAndComparer_ValidReturn()
    {
        // Arrange
        var comparer = (TestObject? a, TestObject? b) => a?.Value == b?.Value;
        var setup = TestSuite.TestSuite.Sequential
            .WithComparer(comparer);
        
        // Act
        var reporter = setup
            .UseOperation((TestObject a, TestObject b) => new TestObject(a.Value + b.Value))
            .Expect(new TestObject(2)).WithInput([new TestObject(1), new TestObject(1)])
            .Run();

        // Assert
        AssertPassedTestResult(1, reporter, new TestObject(2), [new TestObject(1), new TestObject(1)], comparer);
    }
    
    [Fact]
    public void UseOperation_WithCustomObjectWithoutSetOperation_ShouldThrow()
    {
        // Arrange
        
        // Act
        var func = () => TestSuite.TestSuite.Sequential
            .WithComparer<TestObject>((x, y) => x == y)
            .Run();

        // Assert
        Assert.Throws<InvalidOperationException>(func);
    }
    
    [Fact]
    public void TestSuite_Ignored_TestCasesShouldBeIgnored()
    {
        // Arrange
        var builder = TestSuite.TestSuite.Sequential.Ignore
            .UseOperation(TestHelpers.Adder)
            .Expect(2).WithInput(1, 1, 1)
            .Expect(3).WithInput(1, 1, 1);
            
        // Act    
        var reporter = builder.Run();
        
        // Assert
        AssertSkippedTestResult(1, reporter, 2, [1, 1, 1]);
        AssertSkippedTestResult(2, reporter, 3, [1, 1, 1]);
    }
    
    [Fact]
    public void TestSuite_WithDisplayName_ShouldBeCustomDisplayName()
    {
        // Arrange
        var displayName = "test";
        var builder = TestSuite.TestSuite.Sequential.WithDisplayName(displayName).UseOperation(TestHelpers.Adder);
            
        // Act    
        var reporter = builder.Run();
        
        // Assert
        Assert.Equal(displayName, reporter.TestSuiteResult.DisplayName);
    }

    private static void AssertPassedTestResult<TOutput>(
        int testNumber,
        ITestSuiteReporter<TOutput> testSuiteReporter, 
        TOutput? expected, 
        object?[] inputs,
        Func<TOutput?, TOutput?, bool>? comparer = null)
    {
        var testSuiteResult = testSuiteReporter.TestSuiteResult;
        
        Assert.NotNull(testSuiteResult);
        var testCase = testSuiteResult.TestCases
            .FirstOrDefault(x => x.Number == testNumber);
        
        Assert.NotNull(testCase);
        Assert.Equal(AssertStatus.Passed, testCase.AssertStatus);
        Assert.Equal(ValidationStatus.Valid, testCase.ValidationStatus);
        Assert.NotEmpty(testCase.ValidationResults);
        foreach (var validationResult in testCase.ValidationResults)
            Assert.True(validationResult.IsValid);
        
        Assert.NotNull(testCase.Assert);
        Assert.NotNull(testCase.Assert.Output);
        if (typeof(TOutput) == typeof(object) || typeof(IEquatable<TOutput>).IsAssignableFrom(typeof(TOutput)))
        {
            Assert.Equal(expected, testCase.Expected);
            Assert.Equal(inputs, testCase.Inputs);
            Assert.Equal(testCase.Expected, testCase.Assert.Output.Value);
        }
        else
        {
            Assert.True(comparer?.Invoke(expected, testCase.Expected));
            Assert.True(comparer?.Invoke(testCase.Expected, testCase.Assert.Output.Value));
            foreach (var input in inputs.Zip(testCase.Inputs))
                Assert.True(comparer?.Invoke((TOutput?)input.First, (TOutput?)input.Second));
        }
        Assert.True(testCase.Assert.Passed);
        Assert.Null(testCase.Assert.Exception);
        Assert.True(testCase.Assert.ElapsedTime.TotalMilliseconds > 0);
    }
    
    private static void AssertNotPassedTestResult<TOutput>(
        int testNumber,
        ITestSuiteReporter<TOutput> testSuiteReporter, 
        TOutput expected, 
        object[] inputs)
    {
        var testSuiteResult = testSuiteReporter.TestSuiteResult;
        
        var testCase = testSuiteResult.TestCases
            .FirstOrDefault(x => x.Number == testNumber);
        
        Assert.NotNull(testCase);
        Assert.Equal(AssertStatus.NotPassed, testCase.AssertStatus);
        Assert.Equal(ValidationStatus.Valid, testCase.ValidationStatus);
        Assert.NotEmpty(testCase.ValidationResults);
        foreach (var validationResult in testCase.ValidationResults)
            Assert.True(validationResult.IsValid);
        
        Assert.NotNull(testCase.Assert);
        Assert.Equal(expected, testCase.Expected);
        Assert.Equal(inputs, testCase.Inputs);
        Assert.False(testCase.Assert.Passed);
        Assert.Null(testCase.Assert.Exception);
        Assert.NotNull(testCase.Assert.Output);
        Assert.NotEqual(testCase.Expected, testCase.Assert.Output.Value);
        Assert.True(testCase.Assert.ElapsedTime.TotalMilliseconds > 0);
    }

    private void AssertSkippedTestResult<TOutput>(
        int testNumber,
        ITestSuiteReporter<TOutput> testSuiteReporter, 
        TOutput expected, 
        object[] inputs)
    {
        var testSuiteResult = testSuiteReporter.TestSuiteResult;
        
        var testCase = testSuiteResult.TestCases
            .FirstOrDefault(x => x.Number == testNumber);
        
        Assert.NotNull(testCase);
        Assert.Equal(AssertStatus.Unknown, testCase.AssertStatus);
        Assert.Equal(ValidationStatus.Unknown, testCase.ValidationStatus);
        
        Assert.Null(testCase.Assert);
        Assert.Equal(expected, testCase.Expected);
        Assert.Equal(inputs, testCase.Inputs);
    }
    
    [TestSuiteDelegate]
    private static int AdderWithAttribute(int number1, int number2)
    {
        return number1 + number2;
    }
    
    private class EquatableTestObject(int value) : IEquatable<EquatableTestObject>
    {
        public int Value { get; } = value;

        public bool Equals(EquatableTestObject? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EquatableTestObject)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
    
    private class TestObject(int value)
    {
        public int Value { get; } = value;
    }
}