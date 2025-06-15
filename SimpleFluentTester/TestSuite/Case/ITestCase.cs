using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Case;

public interface ITestCase : IValidatedObject
{
    int Number { get; }
    
    IComparedObject[] Inputs { get; }
    
    IComparedObject Expected { get; }
}