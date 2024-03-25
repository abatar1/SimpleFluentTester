using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.UnitTests.TestObjects;

public sealed class TestCaseOperation(Delegate operation, TestCase testCase)
{
    public Delegate Operation { get; } = operation;

    public TestCase TestCase { get; } = testCase;
}