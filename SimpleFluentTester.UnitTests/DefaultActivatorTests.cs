using SimpleFluentTester.Helpers;

namespace SimpleFluentTester.UnitTests;

public class DefaultActivatorTests
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

    private class CustomClass;
}