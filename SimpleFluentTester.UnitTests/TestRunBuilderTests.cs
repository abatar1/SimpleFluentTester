using System.Reflection;
using Moq;
using SimpleFluentTester.Entities;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.UnitTests;

public class TestRunBuilderTests
{
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
        Assert.NotNull(secondTestCase);
        Assert.True(secondTestCase.ShouldBeCalculated);
        Assert.Equal(2, secondTestCase.Expected);
        Assert.Equal([2, 1], secondTestCase.Inputs);
        Assert.True(secondTestCase.LazyResult.IsValueCreated);
        Assert.False(secondTestCase.LazyResult.Value.Passed);
        Assert.Null(secondTestCase.LazyResult.Value.Exception);
        Assert.NotNull(secondTestCase.LazyResult.Value.Output);
        Assert.NotEqual(secondTestCase.Expected, secondTestCase.LazyResult.Value.Output.Value);
        Assert.True(secondTestCase.LazyResult.Value.ElapsedTime.TotalMilliseconds > 0);
        
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
        var builder = new TestRunBuilder<int>(new DefaultTestRunReporterFactory(), new EntryAssemblyProvider());
        
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
        var builder = new TestRunBuilder<int>(new DefaultTestRunReporterFactory(), entryAssemblyProviderMock.Object);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        var testCases = GetTestCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        AssertValidTestCase(firstTestCase, 2, [1, 1]);
    }

    private static void AssertValidTestCase<TOutput>(TestCase<TOutput>? testCase, TOutput expected, object[] inputs)
    {
        Assert.NotNull(testCase);
        Assert.True(testCase.ShouldBeCalculated);
        Assert.Equal(expected, testCase.Expected);
        Assert.Equal(inputs, testCase.Inputs);
        Assert.True(testCase.LazyResult.IsValueCreated);
        Assert.True(testCase.LazyResult.Value.Passed);
        Assert.Null(testCase.LazyResult.Value.Exception);
        Assert.NotNull(testCase.LazyResult.Value.Output);
        Assert.Equal(testCase.Expected, testCase.LazyResult.Value.Output.Value);
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
}