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
        
        var firstIteration = testCases.FirstOrDefault(x => x.Iteration == 1);
        Assert.NotNull(firstIteration);
        Assert.True(firstIteration.ShouldBeCalculated);
        Assert.Equal(2, firstIteration.Expected);
        Assert.Equal([1, 1], firstIteration.Inputs);
        Assert.True(firstIteration.LazyResult.IsValueCreated);
        Assert.True(firstIteration.LazyResult.Value.Passed);
        Assert.Null(firstIteration.LazyResult.Value.Exception);
        Assert.NotNull(firstIteration.LazyResult.Value.Output);
        Assert.Equal(firstIteration.Expected, firstIteration.LazyResult.Value.Output.Value);
        Assert.True(firstIteration.LazyResult.Value.ElapsedTime.TotalMilliseconds > 0);
        
        var secondIteration = testCases.FirstOrDefault(x => x.Iteration == 2);
        Assert.NotNull(secondIteration);
        Assert.True(secondIteration.ShouldBeCalculated);
        Assert.Equal(2, secondIteration.Expected);
        Assert.Equal([2, 1], secondIteration.Inputs);
        Assert.True(secondIteration.LazyResult.IsValueCreated);
        Assert.False(secondIteration.LazyResult.Value.Passed);
        Assert.Null(secondIteration.LazyResult.Value.Exception);
        Assert.NotNull(secondIteration.LazyResult.Value.Output);
        Assert.NotEqual(secondIteration.Expected, secondIteration.LazyResult.Value.Output.Value);
        Assert.True(secondIteration.LazyResult.Value.ElapsedTime.TotalMilliseconds > 0);
        
        var thirdIteration = testCases.FirstOrDefault(x => x.Iteration == 3);
        Assert.NotNull(thirdIteration);
        Assert.False(thirdIteration.ShouldBeCalculated);
        Assert.Equal(3, thirdIteration.Expected);
        Assert.Equal([2, 1], thirdIteration.Inputs);
        Assert.False(thirdIteration.LazyResult.IsValueCreated);
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
        
        var firstIteration = testCases.FirstOrDefault(x => x.Iteration == 1);
        Assert.NotNull(firstIteration);
        Assert.NotNull(firstIteration.LazyResult.Value.Exception);
        Assert.IsType<CustomException>(firstIteration.LazyResult.Value.Exception);
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
        var func1 = () => new TestRunBuilder<int>(null, new DefaultTestRunReporterFactory());
        var func2 = () => new TestRunBuilder<int>(StaticMethods.Adder, null);
        var parameterInfoMock = new Mock<ParameterInfo>();
        var func3 = () => new TestRunBuilder<int>(StaticMethods.Adder, [parameterInfoMock.Object], new DefaultTestRunReporterFactory(), null);
        var func4 = () => new TestRunBuilder<int>(StaticMethods.Adder, [parameterInfoMock.Object], null, new List<TestCase<int>>());
        var func5 = () => new TestRunBuilder<int>(StaticMethods.Adder, null, new DefaultTestRunReporterFactory(), new List<TestCase<int>>());
        
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