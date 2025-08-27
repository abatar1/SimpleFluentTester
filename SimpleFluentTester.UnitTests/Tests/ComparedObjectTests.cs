using System.Reflection;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Tests;

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
        comparedObject.AssertSingleValue(obj);
    }
    
    [Fact]
    public void Wrap_Exception_ShouldBeValid()
    {
        // Assign
        var obj = new Exception();

        // Act
        var comparedObject = ComparedObjectFactory.Wrap(obj);

        // Assert
        comparedObject.AssertSingleException(obj);
    }
     
    [Fact]
    public void Wrap_Parameter_ShouldBeValid()
    {
        // Assign
        var obj = 1;
        var parameter = new DeferredOperationParameter(new Lazy<ParameterInfo>());
            
        // Act
        var comparedObject = ComparedObjectFactory.WrapParameter(1, parameter);

        // Assert
        comparedObject.AssertSingleParameter(obj, parameter);
    }
    
    [Fact]
    public void Wrap_Many_ShouldBeValid()
    {
        // Assign
        var obj1 = 1;
        string? obj2 = null;
        var obj3 = new Exception();
            
        // Act
        var comparedObjects = ComparedObjectFactory.WrapMany([obj1, obj2, obj3]);

        // Assert
        comparedObjects[0].AssertSingleValue(obj1);
        comparedObjects[1].AssertNull();
        comparedObjects[2].AssertSingleException(obj3);
    }
}