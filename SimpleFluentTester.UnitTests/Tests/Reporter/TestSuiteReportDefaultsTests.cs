using Microsoft.Extensions.Logging;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;

namespace SimpleFluentTester.UnitTests.Tests.Reporter;

public sealed class TestSuiteReportDefaultsTests
{
    [Fact]
    public void DetermineLogLevel_InvalidTestCase_ShouldBeError()
    {
        // Assign
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Invalid);
        
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
        var testSuiteResult = TestSuiteFactory.CreateTestSuiteRunResult(testCase: TestCaseExamples.Invalid);
        
        // Act
        var footerString = testSuiteResult.ToFooterString();
        
        // Assert
        var lines = SeparateToLines(footerString);
        Assert.Equal("0/1 test cases have been passed, 1 test case failed", lines[0]);
        Assert.Equal("Non-valid test cases numbers: 1", lines[1]);
        Assert.Equal(2, lines.Count);
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
        Assert.Equal("0/1 test cases have been passed, 1 test case failed", lines[0]);
        Assert.Equal("Not passed test cases numbers: 1", lines[1]);
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
        Assert.Equal("0/1 test cases have been passed, 1 test case failed", lines[0]);
        Assert.Equal("Failed test cases with exceptions numbers: 1", lines[1]);
        Assert.Equal(3, lines.Count);
    }

    [Fact]
    public void ToFormattedString_PassedTestCase_ShouldReturnPassedString()
    {
        // Assign
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var completedTestCase = TestCaseExamples.Passed.CompleteTestCase(container);
        
        // Act
        var formattedString = completedTestCase.ToFormattedString();

        // Assert
        var lines = SeparateToLines(formattedString);
        Assert.Equal("Test case [1] passed", lines[0]);
        Assert.Equal("\tInputs: '1', '2'", lines[1]);
        Assert.Equal("\tExpected: '3'", lines[2]);
        Assert.Equal("\tOutput: '3'", lines[3]);
        Assert.Contains("\tElapsed: ", lines[4]);
        Assert.Equal(5, lines.Count);
    }
    
    [Fact]
    public void ToFormattedString_NotPassedTestCase_ShouldReturnNotPassedString()
    {
        // Assign
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var completedTestCase = TestCaseExamples.NotPassed.CompleteTestCase(container);
        
        // Act
        var formattedString = completedTestCase.ToFormattedString();

        // Assert
        var lines = SeparateToLines(formattedString);
        Assert.Equal("Test case [1] not passed", lines[0]);
        Assert.Equal("\tInputs: '1', '2'", lines[1]);
        Assert.Equal("\tExpected: '4'", lines[2]);
        Assert.Equal("\tOutput: '3'", lines[3]);
        Assert.Contains("\tElapsed: ", lines[4]);
        Assert.Equal(5, lines.Count);
    }
    
    [Fact]
    public void ToFormattedString_NotPassedWithExceptionTestCase_ShouldReturnNotPassedStringWithException()
    {
        // Assign
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var completedTestCase = TestCaseExamples.NotPassedWithOperationException.CompleteTestCase(container);
        
        // Act
        var formattedString = completedTestCase.ToFormattedString();

        // Assert
        var lines = SeparateToLines(formattedString);
        Assert.Equal("Test case [1] not passed with an exception", lines[0]);
        Assert.Equal("Exception: Exception of type 'System.Exception' was thrown.", lines[1]);
        Assert.Equal("\tInputs: '1', '2'", lines[2]);
        Assert.Equal("\tExpected: '3'", lines[3]);
        Assert.Contains("\tElapsed: ", lines[4]);
        Assert.Equal(5, lines.Count);
    }
    
    [Fact]
    public void ToFormattedString_NotValidTestCase_ShouldReturnValidationStrings()
    {
        // Assign
        var container = TestSuiteFactory.CreateEmptyContextContainer();
        var completedTestCase = TestCaseExamples.Invalid.CompleteTestCase(container);
        
        // Act
        var formattedString = completedTestCase.ToFormattedString();

        // Assert
        var lines = SeparateToLines(formattedString);
        Assert.Equal("Test case [1] not passed with a validation error:", lines[0]);
        Assert.Equal("\t-Validation subject: Inputs", lines[1]);
        Assert.Equal("\tError message: Passed parameters and expected operation parameters are not equal.", lines[2]);
        Assert.Equal("Test case [1] not calculated", lines[3]);
        Assert.Equal("\tInputs: 'test', '2'", lines[4]);
        Assert.Equal("\tExpected: '4'", lines[5]);
        Assert.Equal(6, lines.Count);
    }

    private static List<string> SeparateToLines(string str)
    {
        return str.Split(Environment.NewLine)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }
}