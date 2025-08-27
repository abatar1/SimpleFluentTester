using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter.Console;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Tests.Reporter;

public sealed class TestSuiteReportDefaultsTests
{
    [Fact]
    public void DetermineLogLevel_InvalidTestCase_ShouldBeError()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.NonValidOperation);
        
        // Act
        var logLevel = testSuiteResult.DetermineLogLevel();
        
        // Assert
        Assert.Equal(LogLevel.Error, logLevel);
    }
    
    [Fact]
    public void DetermineLogLevel_NotPassedTestCase_ShouldBeError()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.NotPassed);
        
        // Act
        var logLevel = testSuiteResult.DetermineLogLevel();
        
        // Assert
        Assert.Equal(LogLevel.Error, logLevel);
    }
    
    [Fact]
    public void DetermineLogLevel_PassedTestCase_ShouldBeInformation()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Passed);
        
        // Act
        var logLevel = testSuiteResult.DetermineLogLevel();
        
        // Assert
        Assert.Equal(LogLevel.Information, logLevel);
    }
    
    [Fact]
    public void ToFooterString_PassedTestCase_NoValidationStrings()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Passed);
        
        // Act
        var footerString = testSuiteResult.ToFooterString();
        
        // Assert
        var lines = SeparateToLines(footerString);
        Assert.Equal("1/1 tests passed!", lines[0]);
        Assert.Equal(2, lines.Count);
    }
    
    [Fact]
    public void ToFooterString_InvalidTestCase_WithValidationStrings()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.NonValidOperation);
        
        // Act
        var footerString = testSuiteResult.ToFooterString();
        
        // Assert
        var lines = SeparateToLines(footerString);
        Assert.Equal("Passed: 0, Not passed: 0, Failed: 1", lines[0]);
        Assert.Equal("Failed numbers: 1", lines[1]);
        Assert.Equal(3, lines.Count);
    }
    
    [Fact]
    public void ToFooterString_NotPassedTestCase_WithValidationStrings()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.NotPassed);
        
        // Act
        var footerString = testSuiteResult.ToFooterString();
        
        // Assert
        var lines = SeparateToLines(footerString);
        Assert.Equal("Passed: 0, Not passed: 1, Failed: 0", lines[0]);
        Assert.Equal("Not passed numbers: 1", lines[1]);
        Assert.Equal(3, lines.Count);
    }
    
    [Fact]
    public void ToFooterString_NotPassedWithException_WithValidationStrings()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.NotPassedWithOperationException);
        
        // Act
        var footerString = testSuiteResult.ToFooterString();
        
        // Assert
        var lines = SeparateToLines(footerString);
        Assert.Equal("Passed: 0, Not passed: 1, Failed: 0", lines[0]);
        Assert.Equal("Not passed with exception numbers: 1", lines[1]);
        Assert.Equal(3, lines.Count);
    }

    [Fact]
    public void ToFormattedString_PassedTestCase_ShouldReturnPassedString()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        var completedTestCase = TestCaseExamples.Passed.CompleteTestCase(container);
        
        // Act
        var formattedString = completedTestCase.ToFormattedString();

        // Assert
        Assert.Empty(formattedString);
    }
    
    [Fact]
    public void ToFormattedString_NotPassedTestCase_ShouldReturnNotPassedString()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        var completedTestCase = TestCaseExamples.NotPassed.CompleteTestCase(container);
        
        // Act
        var formattedString = completedTestCase.ToFormattedString();

        // Assert
        var lines = SeparateToLines(formattedString);
        Assert.Equal("Test case [1] was not successful", lines[0]);
        Assert.Contains("Reason: Test case not passed", lines[1]);
        Assert.Contains("Inputs: '1', '2'", lines[2]);
        Assert.Contains("Expected: '4'", lines[3]);
        Assert.Contains("Result: '3'", lines[4]);
        Assert.Contains("Elapsed: ", lines[5]);
        Assert.Equal(6, lines.Count);
    }
    
    [Fact]
    public void ToFormattedString_NotPassedWithExceptionTestCase_ShouldReturnNotPassedStringWithException()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        var completedTestCase = TestCaseExamples.NotPassedWithOperationException.CompleteTestCase(container);
        
        // Act
        var formattedString = completedTestCase.ToFormattedString();

        // Assert
        var lines = SeparateToLines(formattedString);
        Assert.Equal("Test case [1] was not successful", lines[0]);
        Assert.Contains("Reason: Test case not passed with an exception", lines[1]);
        Assert.Contains("Exception: Exception of type 'System.Exception' was thrown.", lines[2]);
        Assert.Contains("Inputs: '1', '2'", lines[3]);
        Assert.Contains("Expected: '3'", lines[4]);
        Assert.Contains("Elapsed: ", lines[5]);
        Assert.Equal(6, lines.Count);
    }
    
    [Fact]
    public void ToFormattedString_NotValidTestCase_ShouldReturnValidationStrings()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        var completedTestCase = TestCaseExamples.NonValidOperation.CompleteTestCase(container);
        
        // Act
        var formattedString = completedTestCase.ToFormattedString();

        // Assert
        var lines = SeparateToLines(formattedString);
        Assert.Equal("Test case [1] not passed with a validation error:", lines[0]);
        Assert.Contains("Validation subject: Operation", lines[1]);
        Assert.Contains($"Error message: {TestCaseExamples.NonValidOperationMessage}", lines[2]);
        Assert.Equal("Test case [1] was not successful", lines[3]);
        Assert.Contains("Reason: Test case assertion has failed with an exception", lines[4]);
        Assert.Contains("Inputs: '1', '2'", lines[5]);
        Assert.Contains("Expected: '3'", lines[6]);
        Assert.Contains("Result: 'null'", lines[7]);
        Assert.Contains("Elapsed", lines[8]);
        Assert.Equal(9, lines.Count);
    }

    private static List<string> SeparateToLines(string str)
    {
        return str.Split(Environment.NewLine)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }
}