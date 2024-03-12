using NUnit.Framework;
using SimpleFluentTester.TestCase;

namespace SimpleFluentTester.NUnitRunner.InternalTests;

[TestFixture]
internal sealed class TestCaseTests
{
    [DatapointSource] 
    // ReSharper disable once NotAccessedField.Global
    public static CompletedTestCase<object>[]? Datapoint;

    [Theory]
    public void TestMethod(CompletedTestCase<object> completedTestCase)
    {
        completedTestCase.AssertTestCase();
    }
}