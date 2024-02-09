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
        var builder = SetupBuilder();
            
        // Act    
        var func = () => builder.Expect(2).WithInput(1, 1, 1);
        
        // Assert
        Assert.Throws<TargetParameterCountException>(func);
    }
    
    [Fact]
    public void AddTestCase_ParametersNumberLessThanExpected_ThrowsException()
    {
        // Arrange
        var builder = SetupBuilder();
            
        // Act    
        var func = () => builder.Expect(2).WithInput(1);
        
        // Assert
        Assert.Throws<TargetParameterCountException>(func);
    }
    
    [Fact]
    public void AddTestCase_ParametersWrongType_ThrowsException()
    {
        // Arrange
        var builder = SetupBuilder();
            
        // Act    
        var func = () => builder.Expect(2).WithInput(1, "test");
        
        // Assert
        Assert.Throws<InvalidCastException>(func);
    }
    
    [Fact]
    public void AddTestCase_ParametersValidTypesAndCount_ValidReturn()
    {
        // Arrange
        var builder = SetupBuilder();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(2).WithInput(2, 1)
            .Expect(3).WithInput(2, 1)
            .Run(1, 2);

        // Assert
        var testCases = GetTesCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        Assert.NotNull(firstTestCase);
        Assert.True(firstTestCase.ShouldBeCalculated);
        Assert.Equal(2, firstTestCase.Expected);
        Assert.Equal([1, 1], firstTestCase.Inputs);
        Assert.True(firstTestCase.LazyResult.IsValueCreated);
        Assert.True(firstTestCase.LazyResult.Value.Passed);
        Assert.Null(firstTestCase.LazyResult.Value.Exception);
        Assert.NotNull(firstTestCase.LazyResult.Value.Output);
        Assert.Equal(firstTestCase.Expected, firstTestCase.LazyResult.Value.Output.Value);
        Assert.True(firstTestCase.LazyResult.Value.ElapsedTime.TotalMilliseconds > 0);
        
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
        var builder = TestSuite.Setup()
            .UseOperation<int>(StaticMethods.AdderThrowsCustomException);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        var testCases = GetTesCasesFromReporter(reporter);
        
        var firstTestCase = testCases.FirstOrDefault(x => x.Number == 1);
        Assert.NotNull(firstTestCase);
        Assert.NotNull(firstTestCase.LazyResult.Value.Exception);
        Assert.IsType<CustomException>(firstTestCase.LazyResult.Value.Exception);
    }
    
    [Fact]
    public void Run_InvalidIterationNumber_ThrowsException()
    {
        // Arrange
        var builder1 = SetupBuilder();
        var builder2 = SetupBuilder();
        
        // Act
        var func1 = () => builder1.Expect(2).WithInput(1, 1).Run(2);
        var func2 = () => builder2.Expect(2).WithInput(1, 1).Run(1, 2);
        
        // Assert
        Assert.Throws<InvalidOperationException>(func1);
        Assert.Throws<InvalidOperationException>(func2);
    }
    
    [Fact]
    public void TestRunBuilder_PassNullArgumentsToCtor_ThrowsException()
    {
        // Arrange
        
        // Act
        var func1 = () => new TestRunBuilder<int>(null!, new DefaultTestRunReporterFactory());
        var func2 = () => new TestRunBuilder<int>(StaticMethods.Adder, null!);
        var parameterInfoMock = new Mock<ParameterInfo>();
        var func3 = () => new TestRunBuilder<int>(StaticMethods.Adder, [parameterInfoMock.Object], new DefaultTestRunReporterFactory(), null!);
        var func4 = () => new TestRunBuilder<int>(StaticMethods.Adder, [parameterInfoMock.Object], null!, new List<TestCase<int>>());
        var func5 = () => new TestRunBuilder<int>(StaticMethods.Adder, null!, new DefaultTestRunReporterFactory(), new List<TestCase<int>>());
        
        // Assert
        Assert.Throws<ArgumentNullException>(func1);
        Assert.Throws<ArgumentNullException>(func2);
        Assert.Throws<ArgumentNullException>(func3);
        Assert.Throws<ArgumentNullException>(func4);
        Assert.Throws<ArgumentNullException>(func5);
    }
    
    [Fact]
    public void TestRunBuilder_InvalidDelegateReturnType_ThrowsException()
    {
        // Arrange
        var builder = new TestRunBuilder<int>((int _, int _) => "test", new DefaultTestRunReporterFactory());
        
        // Act
        var func = () => builder.Expect(2).WithInput(1, 1).Run();
        
        // Assert
        Assert.Throws<InvalidCastException>(func);
    }

    private static IList<TestCase<TOutput>> GetTesCasesFromReporter<TOutput>(BaseTestRunReporter<TOutput> reporter)
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

    private static TestRunBuilder<int> SetupBuilder()
    {
        return TestSuite.Setup()
            .UseOperation<int>(StaticMethods.Adder);
    }
}