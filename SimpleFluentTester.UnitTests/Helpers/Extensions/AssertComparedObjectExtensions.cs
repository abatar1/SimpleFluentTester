using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Parameter;

namespace SimpleFluentTester.UnitTests.Helpers.Extensions;

internal static class AssertComparedObjectExtensions
{
    public static void AssertSingleValue(this IList<ITestClause> testClauses, object expectedValue)
    {
        Assert.Single(testClauses);
        var comparedObject = testClauses.TryGetResult();
        comparedObject.AssertSingleValue(expectedValue);
    }
    
    public static void AssertSingleValue(this IComparedObject comparedObject, object expectedValue)
    {
        if (comparedObject is not ValueObject)
        {
            Assert.Fail($"Expected exception result, actual result was {comparedObject.Value}");
            return;
        }
        
        Assert.NotNull(comparedObject);
        Assert.NotNull(comparedObject.Value);
        Assert.NotNull(comparedObject.Type);
        Assert.Equal(ComparedObjectVariety.Value, comparedObject.Variety);
        Assert.Equal(expectedValue, comparedObject.Value);
        Assert.Equal(expectedValue.GetType(), comparedObject.Type);

        var wrappedExpectedValue = ComparedObjectFactory.Wrap(expectedValue);
        Assert.Equal(wrappedExpectedValue.ToString(), comparedObject.ToString());
    }
    
    public static void AssertSingleParameter(this IComparedObject comparedObject, object expectedValue, DeferredOperationParameter parameter)
    {
        if (comparedObject is not ParameterObject parameterObject)
        {
            Assert.Fail($"Expected parameter result, actual result was {comparedObject.Value}");
            return;
        }
        
        Assert.NotNull(parameterObject);
        Assert.NotNull(parameterObject.Value);
        Assert.Null(parameterObject.Type);
        Assert.Equal(ComparedObjectVariety.Parameter, parameterObject.Variety);
        Assert.Equal(parameter, parameterObject.Parameter);
        AssertSingleValue((IComparedObject)parameterObject.Value, expectedValue);
    }
    
    public static void AssertSingleException<TException>(this IList<ITestClause> testClauses, TException expectedException)
        where TException : Exception
    {
        Assert.Single(testClauses);
        var comparedObject = testClauses.TryGetResult();
        comparedObject.AssertSingleException(expectedException);
    }
    
    public static void AssertSingleException<TException>(this IComparedObject comparedObject, TException expectedException)
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
    
    private static IComparedObject TryGetResult(this IList<ITestClause> testClauses)
    {
        var clause = testClauses.First();
        
        if (clause is ExecutedTestClause executedTestClause)
            return executedTestClause.Result;
        if (clause is AssertedTestClause assertedTestClause)
            return assertedTestClause.Result;
        
        Assert.Fail("Test clause is not an ExecutedTestClause or AssertedTestClause");
        throw new InvalidOperationException();
    }
}