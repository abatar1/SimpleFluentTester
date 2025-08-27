using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class TestCaseBuilderTests
{
    [Fact]
    public void TestCaseBuilder_WithValidInput_ShouldBeValid()
    {
        // Assign
        var input = new object[] {1, 2};
        var expected = 3;
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        var clauses = TestClauseFactory.DefineFromValue(expected);
        var testCaseBuilder = new TestCaseBuilder(container, clauses);
        
        // Act
        testCaseBuilder.WithInput(input);

        // Assert
        var testCase = Assert.Single(container.Context.TestCases);
        testCase.AssertValid();
        
        var actualInput = testCase.Inputs.Select(x => x.Value).ToArray();
        Assert.Equal(input, actualInput);
        
        var clause = Assert.Single(testCase.Clauses);
        Assert.Equal(expected, clause.Expected.Value);
        
        Assert.Equal(3, testCase.Validations.Count);
    }
    
    [Fact]
    public void TestCaseBuilder_WithValidInputAndPrecalculatedInvalidValidation_ShouldBeNonValid()
    {
        // Assign
        var input = new object[] {1, 2};
        var expected = 3;
        var testMessage = "test message";
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int x, int y) => x + y);
        var clauses = TestClauseFactory.DefineFromValue(expected);
        var validations = new List<ValidationResult> { ValidationResult.NonValid(ValidationSubject.Operation, testMessage) };
        var testCaseBuilder = new TestCaseBuilder(container, clauses, validations);
        
        // Act
        testCaseBuilder.WithInput(input);

        // Assert
        var testCase = Assert.Single(container.Context.TestCases);
        testCase.AssertNonValid(ValidationSubject.Operation, testMessage);
        
        var actualInput = testCase.Inputs.Select(x => x.Value).ToArray();
        Assert.Equal(input, actualInput);
        
        var clause = Assert.Single(testCase.Clauses);
        Assert.Equal(expected, clause.Expected.Value);
    }
    
    [Fact]
    public void TestCaseBuilder_NumberShouldIncrease_NumberIncreased()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        var clauses = TestClauseFactory.DefineFromValue(null);
        var testCaseBuilder = new TestCaseBuilder(container, clauses);
        
        // Act
        testCaseBuilder.WithInput(1);

        // Assert
        Assert.Equal(1, container.Context.TestCases.Last().Number);
        testCaseBuilder.WithInput(1);
        Assert.Equal(2, container.Context.TestCases.Last().Number);
    }
    
    [Fact]
    public void TestCaseBuilder_And()
    {
        // Assign
        var firstExpected = 1;
        var secondExpected = 2;
        var container = TestSuiteContextContainer.Default();
        var clauses = TestClauseFactory.DefineFromValue(firstExpected);
        var testCaseBuilder = new TestCaseBuilder(container, clauses);
        
        // Act
        testCaseBuilder.And.ExpectReturn(secondExpected).WithInput(1);

        // Assert
        var testCase = Assert.Single(container.Context.TestCases);
        Assert.Equal(2, testCase.Clauses.Count);

        var firstClause = testCase.Clauses[0];
        Assert.Equal(firstExpected, firstClause.Expected.Value);
        var secondClause = testCase.Clauses[1];
        Assert.Equal(secondExpected, secondClause.Expected.Value);
    }
}