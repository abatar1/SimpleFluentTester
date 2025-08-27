using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.TestCase;

/// <summary>
/// Represents a test case that has been asserted, containing details about its execution,
/// validations, results and assertions.
/// </summary>
public sealed class AssertedTestCase(
    IList<AssertedTestClause> clauses,
    ExecutedTestCase executedTestCase
) : ITestCase
{
    public int Number { get; } = executedTestCase.Number;

    public IList<ITestClause> Clauses { get; } = clauses.Cast<ITestClause>().ToList();

    public IList<IComparedObject> Inputs { get; } = executedTestCase.Inputs;

    public TimeSpan ElapsedTime { get; } = executedTestCase.ElapsedTime;

    public IDictionary<ValidationSubject, IList<Lazy<SubjectValidation>>> Validations { get; } = executedTestCase.Validations;
}