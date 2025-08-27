using System.Reflection;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.TestSuite.Parameter;

namespace SimpleFluentTester.UnitTests.Tests;

[Collection("NonParallelCollection")]
public sealed class ExpectParameterFactoryTests
{
    [Fact]
    public void ExpectParameterFactory_GetByPositions_ShouldPass()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        
        // Act
        var parameterX = ExpectParameterFactory.Create(container, 0).LazyParameterInfo.Value;
        var parameterY = ExpectParameterFactory.Create(container, 1).LazyParameterInfo.Value;

        // Assert
        Assert.Equal("y", parameterY.Name);
        Assert.Equal("x", parameterX.Name);
    }
    
    [Fact]
    public void ExpectParameterFactory_GetByNames_ShouldPass()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        
        // Act
        var parameterX = ExpectParameterFactory.Create(container, "x").LazyParameterInfo.Value;
        var parameterY = ExpectParameterFactory.Create(container, "y").LazyParameterInfo.Value;

        // Assert
        Assert.Equal("y", parameterY.Name);
        Assert.Equal("x", parameterX.Name);
    }

    [Fact]
    public void ExpectParameterFactory_GetNotExistedParameter_ShouldThrowKeyNotFoundException()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);

        // Act
        Func<ParameterInfo> func1 = () => ExpectParameterFactory.Create(container, "z").LazyParameterInfo.Value;
        Func<ParameterInfo> func2 = () => ExpectParameterFactory.Create(container, 2).LazyParameterInfo.Value;

        // Assert
        Assert.Throws<KeyNotFoundException>(func1);
        Assert.Throws<KeyNotFoundException>(func2);
    }
    
    [Fact]
    public void ExpectParameterFactory_OperationNotSpecified_ShouldThrowInvalidOperationException()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();

        // Act
        Func<ParameterInfo> func = () => ExpectParameterFactory.Create(container, "x").LazyParameterInfo.Value;

        // Assert
        Assert.Throws<InvalidOperationException>(func);
    }
        
    [Fact]
    public void ExpectParameterFactory_ParamNamesNotSpecified_ShouldPassOnlyByPositions()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int _, int _) => 0);
        
        // Act
        var parameter1 = ExpectParameterFactory.Create(container, 0).LazyParameterInfo.Value;
        var parameter2 = ExpectParameterFactory.Create(container, 1).LazyParameterInfo.Value;

        // Assert
        Assert.Equal("_", parameter1.Name);
        Assert.Equal("_", parameter2.Name);
    }
}