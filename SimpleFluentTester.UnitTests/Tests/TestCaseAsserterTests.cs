using SimpleFluentTester.TestCase.Pipeline;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.TestSuite.Parameter;
using SimpleFluentTester.UnitTests.Helpers;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests;

public sealed class TestCaseAsserterTests
{
    [Fact]
    public void TestCaseAsserter_ExpectedNull_ShouldBePassedWithNull()
    {
        // Assign
        var expected = (int?)null;
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int _, int _) => expected);

        var clauses = TestClauseFactory.DefineFromValue(expected);
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertPassed(expected, [1, 2]);
    }
    
    [Fact]
    public void TestCaseAsserter_ExpectedNull_ShouldNotWithNotNull()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((Func<int, int, int?>)((_, _) => 1));

        var clauses = TestClauseFactory.DefineFromValue(null);
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertNotPassed((int?)null, [1, 2]);
    }
    
    [Fact]
    public void TestCaseAsserter_ExpectedException_ShouldPassWithTheSameMessage()
    {
        // Assign
        var errorMessage = "Test message";
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((Func<int, int, int>)((_, _) => throw new CustomWithMessageException(errorMessage)));

        var clauses = TestClauseFactory.DefineFromValue(new CustomWithMessageException(errorMessage));
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertPassed(new CustomWithMessageException(errorMessage), [1, 2]);
    }
    
    [Fact]
    public void TestCaseAsserter_ExpectedException_ShouldNotPassWithNotTheSameMessage()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((Func<int, int, int>)((_, _) => throw new CustomException()));

        var clauses = TestClauseFactory.DefineFromValue(new CustomWithMessageException("Expected"));
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertNotPassed(new CustomWithMessageException("Expected"), [1, 2]);
    }
    
    [Fact]
    public void TestCaseAsserter_ExpectedValueAndExceptionResult_ShouldNotPassWithException()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((Func<int, int, int>)((_, _) => throw new CustomException()));

        var clauses = TestClauseFactory.DefineFromValue(3);
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertNotPassedWithException(3, [1, 2], typeof(CustomException));
    }
    
    [Fact]
    public void TestCaseAsserter_ExpectedParameterAndExceptionResult_ShouldNotPassWithException()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((Func<int, int, int>)((_, _) => throw new CustomException()));

        var parameter = ExpectParameterFactory.Create(container, 0);
        var clauses = TestClauseFactory.DefineFromParameter(3, parameter);
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertNotPassedWithException(3, [1, 2], typeof(CustomException));
    }
    
    [Fact]
    public void TestCaseAsserter_AsIgnored_ShouldBeIgnored()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int _, int _) => 3);

        var clauses = TestClauseFactory.DefineFromValue(3);
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.AsIgnored();

        // Assert
        assertedTestCase.AssertIgnored();
    }
    
    [Fact]
    public void TestCaseAsserter_AsFailed_ShouldBeFailed()
    {
        // Assign
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int _, int _) => 3);

        var clauses = TestClauseFactory.DefineFromValue(3);
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.AsFailed();

        // Assert
        assertedTestCase.AssertFailed();
    }
    
    [Fact]
    public void TestCaseAsserter_ComparerThrowsException_ShouldBeFailedWithException()
    {
        // Assign
        var exceptionMessage = "Test message";
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((int _, int _) => 3);
        container.WithComparer((int _, int _) => throw new CustomWithMessageException(exceptionMessage));

        var clauses = TestClauseFactory.DefineFromValue(3);
        var testCase = container.DefineTestCase([1, 2], clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertFailedValidation<CustomWithMessageException>(ValidationSubject.Comparer, "Comparer execution failed with an exception.", exceptionMessage);
    }
    
    [Fact]
    public void TestCaseAsserter_NotEquatableTestObjectArrayInput_ShouldBePassedWithComparer()
    {
        // Assign
        // ReSharper disable once CoVariantArrayConversion
        object?[] input = new NotEquatableTestObject[][] { [new NotEquatableTestObject(1)], [new NotEquatableTestObject(2)] };
        var expected = new NotEquatableTestObject(3);
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((NotEquatableTestObject[] x, NotEquatableTestObject[] y) => new NotEquatableTestObject(x[0].Value + y[0].Value));
        container.WithComparer((NotEquatableTestObject? x, NotEquatableTestObject? y) => x?.Value == y?.Value);

        var clauses = TestClauseFactory.DefineFromValue(expected);
        var testCase = container.DefineTestCase(input, clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertPassed(expected, input, (x, y) => x?.Value == y?.Value);
    }
    
    [Fact]
    public void TestCaseAsserter_NotEquatableTestObjectArrayInput_()
    {
        // Assign
        // ReSharper disable once CoVariantArrayConversion
        object?[] input = new NotEquatableTestObject[][] { [new NotEquatableTestObject(1)], [new NotEquatableTestObject(2)] };
        var expected = new NotEquatableTestObject[] { new(1), new(2) };
        var container = TestSuiteContextContainer.Default();
        container.WithOperation((NotEquatableTestObject[] x, NotEquatableTestObject[] y) => x.Concat(y).ToArray());
        container.WithComparer((NotEquatableTestObject? x, NotEquatableTestObject? y) => x?.Value == y?.Value);

        var clauses = TestClauseFactory.DefineFromValue(expected);
        var testCase = container.DefineTestCase(input, clauses);
       
        var executedTestCase = testCase.Execute();

        // Act
        var assertedTestCase = executedTestCase.Assert();

        // Assert
        assertedTestCase.AssertPassed(expected, input, 
#pragma warning disable CS8604 // Possible null reference argument.
            (x, y) => x.Select(s1 => s1.Value).SequenceEqual(y.Select(s2 => s2.Value)));;
#pragma warning restore CS8604 // Possible null reference argument.
    }
}