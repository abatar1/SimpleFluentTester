using System;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public interface IComparedObject
{
    ComparedObjectVariety Variety { get; }
    
    Type? Type { get; } 
    
    object? Value { get; }
}