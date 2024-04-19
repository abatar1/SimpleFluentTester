using SimpleFluentTester.TestSuite.ComparedObject;

namespace SimpleFluentTester.UnitTests.Extensions;

public static class ComparedObjectExtensions
{
    public static void AssertValue(this IComparedObject comparedObject, object expectedValue)
    {
        Assert.NotNull(comparedObject);
        Assert.NotNull(comparedObject.Value);
        Assert.NotNull(comparedObject.Type);
        Assert.Equal(ComparedObjectVariety.Value, comparedObject.Variety);
        Assert.Equal(expectedValue, comparedObject.Value);
        Assert.Equal(expectedValue.GetType(), comparedObject.Type);
        Assert.Equal(comparedObject.Value.ToString(), comparedObject.ToString());
    }
    
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static void AssertException<TException>(this IComparedObject comparedObject, TException exception)
        where TException : Exception
    {
        Assert.NotNull(comparedObject);
        Assert.NotNull(comparedObject.Value);
        Assert.NotNull(comparedObject.Type);
        Assert.Equal(ComparedObjectVariety.Exception, comparedObject.Variety);
        Assert.Equal(exception, comparedObject.Value);
        Assert.Equal(typeof(TException), comparedObject.Type);
        Assert.Equal($"Exception {comparedObject.Type}", comparedObject.ToString());
    }

    public static void AssertNull(this IComparedObject comparedObject)
    {
        Assert.NotNull(comparedObject);
        Assert.Null(comparedObject.Value);
        Assert.Null(comparedObject.Type);
        Assert.Equal(ComparedObjectVariety.Null, comparedObject.Variety);
        Assert.Equal("null", comparedObject.ToString());
    }
}