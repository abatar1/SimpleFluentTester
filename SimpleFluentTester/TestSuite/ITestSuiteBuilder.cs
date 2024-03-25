using System;
using SimpleFluentTester.Reporter;
using SimpleFluentTester.TestSuite.Case;

namespace SimpleFluentTester.TestSuite;

/// <inheritdoc cref="TestSuiteBuilder"/>
public interface ITestSuiteBuilder
{
    /// <inheritdoc cref="TestSuiteBuilder.Expect"/>
    ITestCaseBuilder Expect(object? expected);

    /// <inheritdoc cref="TestSuiteBuilder.ExpectException{TException}"/>
    ITestCaseBuilder ExpectException<TException>(string? message = null)
        where TException : Exception;

    /// <inheritdoc cref="TestSuiteBuilder.UseOperation"/>
    ITestSuiteBuilder UseOperation(Delegate operation);

    /// <inheritdoc cref="TestSuiteBuilder.WithDisplayName"/>
    ITestSuiteBuilder WithDisplayName(string displayName);

    /// <inheritdoc cref="TestSuiteBuilder.WithComparer{TExpected}"/>
    ITestSuiteBuilder WithComparer<TExpected>(Func<TExpected?, TExpected?, bool> comparer);

    /// <inheritdoc cref="TestSuiteBuilder.Ignore"/>
    ITestSuiteBuilder Ignore { get; }

    /// <inheritdoc cref="TestSuiteBuilder.Run"/>
    ITestSuiteReporter Run(params int[] testNumbers);
}