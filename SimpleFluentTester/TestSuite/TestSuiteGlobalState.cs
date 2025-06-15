using System;
using System.Collections.Generic;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.TestSuite;

internal static class TestSuiteGlobalState
{
    private static readonly HashSet<int> IgnoredTestSuiteNumbers = new();
    private static readonly HashSet<string> IgnoredTestSuiteNames = new();
    private static readonly HashSet<int> AllowedTestSuiteNumbers = new();
    private static readonly HashSet<string> AllowedTestSuiteNames = new();
    
    public static void Ignore(params int[] testSuiteNumbers)
    {
        ThrowIfAlreadyAllowed();

        foreach (var testSuiteNumber in testSuiteNumbers)
            IgnoredTestSuiteNumbers.Add(testSuiteNumber);
    }
    
    public static void Ignore(params string[] testSuiteNames)
    {
        ThrowIfAlreadyAllowed();
        
        foreach (var testSuiteName in testSuiteNames)
            IgnoredTestSuiteNames.Add(testSuiteName);
    }
    
    public static void Allow(params int[] testSuiteNumbers)
    {
        ThrowIfAlreadyIgnored();
        
        foreach (var testSuiteNumber in testSuiteNumbers)
            AllowedTestSuiteNumbers.Add(testSuiteNumber);
    }
    
    public static void Allow(params string[] testSuiteNames)
    {
        ThrowIfAlreadyIgnored();
        
        foreach (var testSuiteName in testSuiteNames)
            AllowedTestSuiteNames.Add(testSuiteName);
    }

    public static bool IsAllowed(ITestSuiteContext context)
    {
        if (IgnoredTestSuiteNumbers.Contains(context.Number))
            return false;
        
        if (IgnoredTestSuiteNames.Contains(context.Name))
            return false;

        if (AllowedTestSuiteNumbers.Count == 0 && AllowedTestSuiteNames.Count == 0)
            return true;

        if (AllowedTestSuiteNumbers.Contains(context.Number))
            return true;
        
        if (AllowedTestSuiteNames.Contains(context.Name))
            return true;

        return false;
    }

    private static void ThrowIfAlreadyAllowed()
    {
        if (AllowedTestSuiteNumbers.Count != 0 || AllowedTestSuiteNames.Count != 0)
            throw new InvalidOperationException("You should call only one of Ignore() or Allow(), not both");
    }
    
    private static void ThrowIfAlreadyIgnored()
    {
        if (IgnoredTestSuiteNumbers.Count != 0 || IgnoredTestSuiteNames.Count != 0)
            throw new InvalidOperationException("You should call only one of Ignore() or Allow(), not both");
    }
}