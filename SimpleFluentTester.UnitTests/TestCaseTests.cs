using System.Reflection;
using SimpleFluentTester.Entities;

namespace SimpleFluentTester.UnitTests;

public class TestCaseTests
{
    [Fact]
    public void TestCase_CreateMultipleInputsAndToString_ShouldNotThrow()
    {
        // Assign
        var lazyResult = new Lazy<CalculatedTestResult<int>>(new CalculatedTestResult<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero));
        _ = lazyResult.Value;
        var testCase = new TestCase<int>(new[] { 1, 2 }.Cast<object>().ToArray(), 3, lazyResult, true, 1);

        // Act
        var printedResult = testCase.ToString();

        // Assert
        Assert.NotEmpty(printedResult);
    }
    
    [Fact]
    public void TestCase_CreateSingleInputAndToString_ShouldNotThrow()
    {
        // Assign
        var lazyResult = new Lazy<CalculatedTestResult<int>>(new CalculatedTestResult<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero));
        _ = lazyResult.Value;
        var testCase = new TestCase<int>(new[] { 1 }.Cast<object>().ToArray(), 3, lazyResult, true, 1);

        // Act
        var printedResult = testCase.ToString();

        // Assert
        Assert.NotEmpty(printedResult);
    }
    
    [Fact]
    public void TestCase_ValueNotCreated_ShouldNotThrow()
    {
        // Assign
        var lazyResult = new Lazy<CalculatedTestResult<int>>(new CalculatedTestResult<int>(true, new ValueWrapper<int>(2),
            new TargetInvocationException(new Exception()), TimeSpan.Zero));
        var testCase = new TestCase<int>(new[] { 1 }.Cast<object>().ToArray(), 3, lazyResult, true, 1);

        // Act
        var printedResult = testCase.ToString();

        // Assert
        Assert.NotEmpty(printedResult);
    }
}