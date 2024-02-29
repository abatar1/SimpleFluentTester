using System.Reflection;
using Moq;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.UnitTests;

public class TestRunBuilderTests
{
    [Fact]
    public void AddTestCase_NoReturnTypeSpecifiedWithValidExpectedType_ValidReturn()
    {
        // Arrange
        var builder = TestSuite.UseOperation(StaticMethods.Adder);
            
        // Act    
        var reporter = builder.Expect(2).WithInput(1, 1).Run();
        
        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        AssertValidTestCase(firstTestCase, 2, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_NoReturnTypeSpecifiedWithInvalidExpectedType_ThrowsException()
    {
        // Arrange
        var builder = TestSuite.UseOperation(StaticMethods.Adder);
            
        // Act    
        var func = () => builder.Expect("123").WithInput(1, 1).Run();
        
        // Assert
        Assert.Throws<InvalidCastException>(func);
    }
    
    [Fact]
    public void AddTestCase_OperationWithNullableParameter_ValidReturn()
    {
        // Arrange
        var builder = TestSuite.UseOperation((int? a, int b) => a == null ? null : a + b);
            
        // Act    
        var reporter = builder.Expect(null).WithInput(null, 1).Run();
        
        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        AssertValidTestCase(firstTestCase, null, [null, 1]);
    }
    
    [Fact]
    public void AddTestCase_OperationWithNullableParameterButActualValue_ValidReturn()
    {
        // Arrange
        var builder = TestSuite.UseOperation((int? a, int? b) => a == null ? null : a + b);
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(3).WithInput(1, 1)
            .Run();
        
        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        AssertValidTestCase(firstTestCase, 2, [1, 1]);
        
        var secondTestCase = testCases.FirstOrDefault(x => x.Number == 2);
        AssertInvalidTestCase(secondTestCase, 3, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_OperationWithoutNullableParameterButNullExpected_ThrowsException()
    {
        // Arrange
        var builder = TestSuite.UseOperation((int a, int b) => a + b);
            
        // Act    
        var func = () => builder.Expect(null).WithInput(1, 1).Run();
        
        // Assert
        Assert.Throws<InvalidCastException>(func);
    }
    
    [Fact]
    public void AddTestCase_ParametersNumberMoreThanExpected_ThrowsException()
    {
        // Arrange
        var builder = SetupAdderBuilder();
            
        // Act    
        var func = () => builder.Expect(2).WithInput(1, 1, 1).Run();
        
        // Assert
        Assert.Throws<TargetParameterCountException>(func);
    }
    
    [Fact]
    public void AddTestCase_ParametersNumberLessThanExpected_ThrowsException()
    {
        // Arrange
        var builder = SetupAdderBuilder();
            
        // Act    
        var func = () => builder.Expect(2).WithInput(1).Run();
        
        // Assert
        Assert.Throws<TargetParameterCountException>(func);
    }
    
    [Fact]
    public void AddTestCase_ParametersWrongType_ThrowsException()
    {
        // Arrange
        var builder = SetupAdderBuilder();
            
        // Act    
        var func = () => builder.Expect(2).WithInput(1, "test").Run();
        
        // Assert
        Assert.Throws<InvalidCastException>(func);
    }
    
    [Fact]
    public void AddTestCase_ParametersValidTypesAndCount_ValidReturn()
    {
        // Arrange
        var builder = SetupAdderBuilder();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(2).WithInput(2, 1)
            .Expect(3).WithInput(2, 1)
            .Run(1, 2);

        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        AssertValidTestCase(firstTestCase, 2, [1, 1]);
        
        var secondTestCase = testCases.FirstOrDefault(x => x.Number == 2);
        AssertInvalidTestCase(secondTestCase, 2, [2, 1]);
        
        var thirdTestCase = testCases.FirstOrDefault(x => x.Number == 3);
        Assert.NotNull(thirdTestCase);
        Assert.False(thirdTestCase.ShouldBeCalculated);
        Assert.Equal(3, thirdTestCase.Expected);
        Assert.Equal([2, 1], thirdTestCase.Inputs);
        Assert.False(thirdTestCase.LazyResult.IsValueCreated);
    }

    [Fact]
    public void AddTestCase_TestingOperationIsBroken_TestCaseHasException()
    {
        // Arrange
        var builder = TestSuite.WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.AdderThrowsCustomException);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        Assert.NotNull(firstTestCase);
        Assert.NotNull(firstTestCase.LazyResult.Value.Exception);
        Assert.IsType<CustomException>(firstTestCase.LazyResult.Value.Exception);
    }
    
    [Fact]
    public void Run_InvalidIterationNumber_ThrowsException()
    {
        // Arrange
        var builder1 = SetupAdderBuilder();
        var builder2 = SetupAdderBuilder();
        
        // Act
        var func1 = () => builder1.Expect(2).WithInput(1, 1).Run(2);
        var func2 = () => builder2.Expect(2).WithInput(1, 1).Run(1, 2);
        
        // Assert
        Assert.Throws<InvalidOperationException>(func1);
        Assert.Throws<InvalidOperationException>(func2);
    }
    
    [Fact]
    public void TestRunBuilder_InvalidDelegateReturnType_ThrowsException()
    {
        // Arrange
        var builder = new TestRunBuilder<int>(new DefaultTestRunReporterFactory(), new EntryAssemblyProvider(), new DefaultActivator(), null);
        
        // Act
        var func = () => builder.UseOperation((int _, int _) => "test").Expect(2).WithInput(1, 1).Run();
        
        // Assert
        Assert.Throws<InvalidCastException>(func);
    }
    
    [Fact]
    public void AddTestCase_TestingOperationNotSet_AdderWithAttributeShouldBeSelected()
    {
        // Arrange
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock
            .Setup(x => x.Get())
            .Returns(Assembly.GetAssembly(typeof(TestRunBuilderTests)));
        var builder = new TestRunBuilder<int>(new DefaultTestRunReporterFactory(), entryAssemblyProviderMock.Object, new DefaultActivator(),null);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        AssertValidTestCase(firstTestCase, 2, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_WithCustomEquatableObject_ValidReturn()
    {
        // Arrange
        var builder = TestSuite
            .WithExpectedReturnType<EquatableTestObject>()
            .UseOperation((EquatableTestObject a, EquatableTestObject b) => new EquatableTestObject(a.Value + b.Value));
        
        // Act
        var reporter = builder
            .Expect(new EquatableTestObject(2)).WithInput(new EquatableTestObject(1), new EquatableTestObject(1))
            .Run();
        
        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        AssertValidTestCase(firstTestCase, new EquatableTestObject(2), [new EquatableTestObject(1), new EquatableTestObject(1)]);
    }
    
    [Fact]
    public void UseOperation_WithCustomObjectAndComparer_ValidReturn()
    {
        // Arrange
        var comparer = (TestObject a, TestObject b) => a.Value == b.Value;
        var setup = TestSuite
            .WithExpectedReturnType(comparer);
        
        // Act
        var reporter = setup
            .UseOperation((TestObject a, TestObject b) => new TestObject(a.Value + b.Value))
            .Expect(new TestObject(2)).WithInput([new TestObject(1), new TestObject(1)])
            .Run();

        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        AssertValidTestCase(firstTestCase, new TestObject(2), [new TestObject(1), new TestObject(1)], comparer);
    }
    
    [Fact]
    public void UseOperation_WithCustomObject_ShouldThrow()
    {
        // Arrange
        
        // Act
        var func = () => TestSuite.WithExpectedReturnType<TestObject>().Run();

        // Assert
        Assert.Throws<InvalidOperationException>(func);
    }
    
    [Fact]
    public void TestSuite_Ignored_TestCasesShouldBeIgnored()
    {
        // Arrange
        var builder = TestSuite.Ignore.WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder)
            .Expect(2).WithInput(1, 1, 1)
            .Expect(3).WithInput(1, 1, 1);
            
        // Act    
        var reporter = builder.Run();
        
        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        var firstTestCase = testCases.First(x => x.Number == 1);
        Assert.False(firstTestCase.ShouldBeCalculated);
        var secondTestCase = testCases.First(x => x.Number == 2);
        Assert.False(secondTestCase.ShouldBeCalculated); 
    }

    private static void AssertValidTestCase<TOutput>(TestCase<TOutput>? testCase, 
        TOutput expected, 
        object[] inputs,
        Func<TOutput, TOutput, bool>? comparer = null)
    {
        Assert.NotNull(testCase);
        Assert.True(testCase.ShouldBeCalculated);
        Assert.True(testCase.LazyResult.IsValueCreated);
        Assert.NotNull(testCase.LazyResult.Value.Output);
        if (typeof(TOutput) == typeof(object) || typeof(IEquatable<TOutput>).IsAssignableFrom(typeof(TOutput)))
        {
            Assert.Equal(expected, testCase.Expected);
            Assert.Equal(inputs, testCase.Inputs);
            Assert.Equal(testCase.Expected, testCase.LazyResult.Value.Output.Value);
        }
        else
        {
            Assert.True(comparer?.Invoke(expected, testCase.Expected));
            Assert.True(comparer?.Invoke(testCase.Expected, testCase.LazyResult.Value.Output.Value));
            foreach (var input in inputs.Zip(testCase.Inputs))
                Assert.True(comparer?.Invoke((TOutput)input.First, (TOutput)input.Second));
        }
        Assert.True(testCase.LazyResult.Value.Passed);
        Assert.Null(testCase.LazyResult.Value.Exception);
        Assert.True(testCase.LazyResult.Value.ElapsedTime.TotalMilliseconds > 0);
    }
    
    private static void AssertInvalidTestCase<TOutput>(TestCase<TOutput>? testCase, 
        TOutput expected, 
        object[] inputs)
    {
        Assert.NotNull(testCase);
        Assert.True(testCase.ShouldBeCalculated);
        Assert.Equal(expected, testCase.Expected);
        Assert.Equal(inputs, testCase.Inputs);
        Assert.True(testCase.LazyResult.IsValueCreated);
        Assert.False(testCase.LazyResult.Value.Passed);
        Assert.Null(testCase.LazyResult.Value.Exception);
        Assert.NotNull(testCase.LazyResult.Value.Output);
        Assert.NotEqual(testCase.Expected, testCase.LazyResult.Value.Output.Value);
        Assert.True(testCase.LazyResult.Value.ElapsedTime.TotalMilliseconds > 0);
    }

    private static IList<TestCase<TOutput>> GetTestCasesFromReporter<TOutput>(BaseTestRunReporter<TOutput> reporter)
    {
        var baseReporterType = reporter.GetType().BaseType;
        Assert.NotNull(baseReporterType);
        var reporterFields = baseReporterType
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        
        var testCasesProperty = reporterFields
            .FirstOrDefault(x => x.FieldType == typeof(IList<TestCase<TOutput>>));
        Assert.NotNull(testCasesProperty);
        var testCases = testCasesProperty.GetValue(reporter) as IList<TestCase<TOutput>>;
        Assert.NotNull(testCases);
        return testCases;
    }

    private static TestRunBuilder<int> SetupAdderBuilder()
    {
        return TestSuite.WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder);
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
            if (obj.GetType() != this.GetType()) return false;
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