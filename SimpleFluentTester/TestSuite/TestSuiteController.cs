using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite;

internal sealed class TestSuiteController : ITestSuiteController
{
    private readonly HashSet<int> _ignoredTestSuiteNumbers = new();
    private readonly HashSet<string> _ignoredTestSuiteNames = new();
    private readonly HashSet<int> _allowedTestSuiteNumbers = new();
    private readonly HashSet<string> _allowedTestSuiteNames = new();
    
    public void Ignore(params int[] testSuiteNumbers)
    {
        ThrowIfAlreadyAllowed();

        foreach (var testSuiteNumber in testSuiteNumbers)
            _ignoredTestSuiteNumbers.Add(testSuiteNumber);
    }
    
    public void Ignore(params string[] testSuiteNames)
    {
        ThrowIfAlreadyAllowed();
        
        foreach (var testSuiteName in testSuiteNames)
            _ignoredTestSuiteNames.Add(testSuiteName);
    }
    
    public void Allow(params int[] testSuiteNumbers)
    {
        ThrowIfAlreadyIgnored();
        
        foreach (var testSuiteNumber in testSuiteNumbers)
            _allowedTestSuiteNumbers.Add(testSuiteNumber);
    }
    
    public void Allow(params string[] testSuiteNames)
    {
        ThrowIfAlreadyIgnored();
        
        foreach (var testSuiteName in testSuiteNames)
            _allowedTestSuiteNames.Add(testSuiteName);
    }

    public bool IsAllowed(ITestSuiteContext context)
    {
        if (_ignoredTestSuiteNumbers.Contains(context.Number))
            return false;
        
        if (_ignoredTestSuiteNames.Contains(context.Name))
            return false;

        if (_allowedTestSuiteNumbers.Count == 0 && _allowedTestSuiteNames.Count == 0)
            return true;

        if (_allowedTestSuiteNumbers.Contains(context.Number))
            return true;
        
        if (_allowedTestSuiteNames.Contains(context.Name))
            return true;

        return false;
    }

    private void ThrowIfAlreadyAllowed()
    {
        if (_allowedTestSuiteNumbers.Count != 0 || _allowedTestSuiteNames.Count != 0)
            throw new InvalidOperationException("You should call only one of Ignore() or Allow(), not both");
    }
    
    private void ThrowIfAlreadyIgnored()
    {
        if (_ignoredTestSuiteNumbers.Count != 0 || _ignoredTestSuiteNames.Count != 0)
            throw new InvalidOperationException("You should call only one of Ignore() or Allow(), not both");
    }
}