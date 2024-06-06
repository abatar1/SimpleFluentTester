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
    
    public static void AssertException<TException>(this IComparedObject comparedObject, TException expectedException)
        where TException : Exception
    {
        if (comparedObject is not ExceptionObject exceptionObject)
        {
            Assert.Fail($"Expected exception result, actual result was {comparedObject.Value}");
            return;
        }

        var actualException = (Exception) exceptionObject.Value;
            
        Assert.NotNull(comparedObject);
        Assert.NotNull(actualException);
        Assert.NotNull(comparedObject.Type);
        Assert.Equal(ComparedObjectVariety.Exception, comparedObject.Variety);
        Assert.Equal(expectedException.Message, actualException.Message);
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