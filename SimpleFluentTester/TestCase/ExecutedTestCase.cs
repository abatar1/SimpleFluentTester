using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase;

/// <summary>
/// Represents a test case that has been executed, containing details about its execution,
/// validations, and results.
/// </summary>
public sealed class ExecutedTestCase(
    IList<ExecutedTestClause> clauses,
    TimeSpan elapsedTime,
    ITestCase testCase
    ) : ITestCase
{
    public int Number { get; } = testCase.Number;

    public IList<ITestClause> Clauses { get; } = clauses.Cast<ITestClause>().ToList();

    public IList<IComparedObject> Inputs { get; } = testCase.Inputs;

    public TimeSpan ElapsedTime { get; } = elapsedTime;

    public IDictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> Validations { get; } = testCase.Validations;
}