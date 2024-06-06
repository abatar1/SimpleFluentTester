using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.UnitTests.Extensions;

namespace SimpleFluentTester.UnitTests.Tests;

public sealed class ComparedObjectFactoryTests
{
    [Fact]
    public void Wrap_NullObject_ShouldBeValid()
    {
        // Assign
        
        // Act
        var comparedObject = ComparedObjectFactory.Wrap(null);

        // Assert
        comparedObject.AssertNull();
    }
    
    [Fact]
    public void Wrap_Value_ShouldBeValid()
    {
        // Assign

        // Act
        var obj = 1;
        var comparedObject = ComparedObjectFactory.Wrap(obj);

        // Assert
        comparedObject.AssertValue(obj);
    }
    
    [Fact]
    public void Wrap_Exception_ShouldBeValid()
    {
        // Assign

        // Act
        var obj = new Exception();
        var comparedObject = ComparedObjectFactory.Wrap(obj);

        // Assert
        comparedObject.AssertException(obj);
        Assert.NotNull(comparedObject);
        Assert.Equal(obj, comparedObject.Value);
        Assert.Equal(obj.GetType(), comparedObject.Type);
        Assert.Equal(ComparedObjectVariety.Exception, comparedObject.Variety);
    }
}