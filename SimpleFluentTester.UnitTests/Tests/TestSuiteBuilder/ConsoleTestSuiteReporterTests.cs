using Moq;
using SimpleFluentTester.Reporter.Console;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class ConsoleTestSuiteReporterTests
{
    [Fact]
    public void ConsoleReporter_SimpleMock_ShouldNotThrow()
    {
        // Assign
        var resultMock = new Mock<ITestSuiteRunResult>();
        var reporter = new ConsoleTestSuiteReporter(resultMock.Object);

        // Act
        reporter.Report();

        // Assert
        resultMock.Verify(x => x.DisplayName, Times.Once);
    }
}