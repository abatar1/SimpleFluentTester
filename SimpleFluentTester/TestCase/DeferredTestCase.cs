using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.TestCase;

/// <summary>
/// Represents a deferred test case constructed with lazily evaluated operations and comparers.
/// </summary>
public sealed class DefinedTestCase(
    Lazy<Delegate?> operationFactory,
    Lazy<Delegate?> comparerFactory,
    IList<IComparedObject> inputs,
    IList<DefinedTestClause> clauses,
    int number) : ITestCase
{
    public int Number { get; } = number;

    public IList<ITestClause> Clauses { get; } = clauses.Cast<ITestClause>().ToList();

    public IList<IComparedObject> Inputs { get; } = inputs;

    public Lazy<Delegate?> OperationFactory { get; } = operationFactory;
    
    public Lazy<Delegate?> ComparerFactory { get; } = comparerFactory;
    
    public IDictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> Validations { get; } =
        new Dictionary<ValidationSubject, IList<Lazy<SubjectValidation>>>();
}