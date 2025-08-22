using System.Collections.Generic;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase;

/// <summary>
/// Defines the contract for a test case used in the context of validation and testing logic.
/// </summary>
/// <remarks>
/// The <see cref="ITestCase"/> interface represents a unit of testing that contains all necessary data
/// required to execute and validate a test scenario. It is primarily utilized within test suites
/// to organize and structure testing operations.
/// Implementations of this interface may define additional behavior or properties to support specific
/// testing requirements.
/// </remarks>
public interface ITestCase : IValidatedObject
{
    int Number { get; }
    
    IList<ITestClause> Clauses { get; }
    
    IList<IComparedObject> Inputs { get; }
}