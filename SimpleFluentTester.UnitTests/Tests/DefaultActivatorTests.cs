using SimpleFluentTester.Helpers;

namespace SimpleFluentTester.UnitTests.Tests;

public sealed class DefaultActivatorTests
{
    [Fact]
    public void CreateInstance_NotNullType_ShouldBeValid()
    {
        // Assign
        var defaultActivator = new DefaultActivator();
        
        // Act
        var resultInstance = defaultActivator.CreateInstance(typeof(CustomClass));

        // Assert
        Assert.NotNull(resultInstance);
    }
    
    [Fact]
    public void CreateInstance_WithParameters_ShouldBeValid()
    {
        // Assign
        var defaultActivator = new DefaultActivator();
        
        // Act
        var resultInstance = defaultActivator.CreateInstance(typeof(CustomClassWithParams), "123");

        // Assert
        Assert.NotNull(resultInstance);
    }

    private class CustomClass;
    
    private class CustomClassWithParams(object parameter);
}