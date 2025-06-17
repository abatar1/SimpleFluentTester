using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Tests;

// todo add parameters
public sealed class ComparedObjectFactoryTests
{
    [Fact]
    public void Wrap_NullObject_ShouldBeValid()
    {
        // Assign
        string? obj = null;
        
        // Act
        var comparedObject = ComparedObjectFactory.Wrap<object>(obj);

        // Assert
        comparedObject.AssertNull();
    }
    
    [Fact]
    public void Wrap_Value_ShouldBeValid()
    {
        // Assign
        var obj = 1;

        // Act
        var comparedObject = ComparedObjectFactory.Wrap(obj);

        // Assert
        comparedObject.AssertValue(obj);
    }
    
    [Fact]
    public void Wrap_Exception_ShouldBeValid()
    {
        // Assign
        var obj = new Exception();

        // Act
        var comparedObject = ComparedObjectFactory.Wrap(obj);

        // Assert
        comparedObject.AssertException(obj);
        Assert.NotNull(comparedObject);
        Assert.Equal(obj, comparedObject.Value);
        Assert.Equal(obj.GetType(), comparedObject.Type);
        Assert.Equal(ComparedObjectVariety.Exception, comparedObject.Variety);
    }
}