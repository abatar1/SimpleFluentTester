﻿using System.Reflection;
using Moq;
using SimpleFluentTester.Entities;
using SimpleFluentTester.TestRun;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests;

public class TestRunBuilderTests
{
    [Fact]
    public void AddTestCase_NoReturnTypeSpecifiedWithValidExpectedType_ShouldBePassed()
    {
        // Arrange
        var builder = TestSuite.Sequential.UseOperation(StaticMethods.Adder);
            
        // Act    
        var reporter = builder.Expect(2).WithInput(1, 1).Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertPassedTestResult(1, testRunResult, 2, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_NoReturnTypeSpecifiedWithInvalidExpectedType_ThrowsException()
    {
        // Arrange
        var builder = TestSuite.Sequential.UseOperation(StaticMethods.Adder);
            
        // Act    
        var reporter = builder.Expect("123").WithInput(1, 1).Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertTestCasesValidationHasFailed(testRunResult, ValidationSubject.Operation);
    }

    private static void AssertContextValidationHasFailed<TOutput>(TestRunResult<TOutput> testRunResult, ValidationSubject validationSubject)
    {
        var validationResult = testRunResult.ContextValidationResults
            .SingleOrDefault(x => x.ValidationSubject == validationSubject);
        
        Assert.NotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.NotNull(validationResult.Message);
        Assert.Equal(validationSubject, validationResult.ValidationSubject);
    }
    
    private static void AssertTestCasesValidationHasFailed<TOutput>(TestRunResult<TOutput> testRunResult, ValidationSubject validationSubject)
    {
        var validationResult = testRunResult.ValidatedTestCases
            .SelectMany(x => x.ValidationResults)
            .SingleOrDefault(x => x.ValidationSubject == validationSubject);
        
        Assert.NotNull(validationResult);
        Assert.False(validationResult.IsValid);
        Assert.NotNull(validationResult.Message);
        Assert.Equal(validationSubject, validationResult.ValidationSubject);
    }
    
    [Fact]
    public void AddTestCase_OperationWithNullableParameter_ShouldBePassed()
    {
        // Arrange
        var builder = TestSuite.Sequential.UseOperation((int? a, int b) => a == null ? null : a + b);
            
        // Act    
        var reporter = builder.Expect(null).WithInput(null, 1).Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertPassedTestResult(1, testRunResult, null, [null, 1]);
    }
    
    [Fact]
    public void AddTestCase_OperationWithNullableParameterButActualValue_ShouldNotThrow()
    {
        // Arrange
        var builder = TestSuite.Sequential.UseOperation((int? a, int? b) => a == null ? null : a + b);
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(3).WithInput(1, 1)
            .Run(2);
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        //AssertPassedTestResult(1, testRunResult, 2, [1, 1]);
        AssertNotPassedTestResult(2, testRunResult, 3, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_OperationWithoutNullableParameterButNullExpected_ThrowsException()
    {
        // Arrange
        var builder = TestSuite.Sequential.UseOperation((int a, int b) => a + b);
            
        // Act    
        var reporter = builder.Expect(null).WithInput(1, 1).Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertTestCasesValidationHasFailed(testRunResult, ValidationSubject.Operation);
    }
    
    [Fact]
    public void AddTestCase_ParametersNumberMoreThanExpected_ThrowsException()
    {
        // Arrange
        var builder = SetupAdderBuilder();
            
        // Act    
        var reporter = builder.Expect(2).WithInput(1, 1, 1).Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertTestCasesValidationHasFailed(testRunResult, ValidationSubject.Inputs);
    }
    
    [Fact]
    public void AddTestCase_ParametersNumberLessThanExpected_ThrowsException()
    {
        // Arrange
        var builder = SetupAdderBuilder();
            
        // Act    
        var reporter = builder.Expect(2).WithInput(1).Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertTestCasesValidationHasFailed(testRunResult, ValidationSubject.Inputs);
    }
    
    [Fact]
    public void AddTestCase_ParametersWrongType_ThrowsException()
    {
        // Arrange
        var builder = SetupAdderBuilder();
            
        // Act    
        var reporter = builder.Expect(2).WithInput(1, "test").Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertTestCasesValidationHasFailed(testRunResult, ValidationSubject.Inputs);
    }
    
    [Fact]
    public void AddTestCase_ParametersValidTypesAndCount_ValidReturn()
    {
        // Arrange
        var builder = SetupAdderBuilder();
            
        // Act    
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Expect(2).WithInput(2, 1)
            .Expect(3).WithInput(2, 1)
            .Run(1, 2);

        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertPassedTestResult(1, testRunResult, 2, [1, 1]);
        AssertNotPassedTestResult(2, testRunResult, 2, [2, 1]);
        AssertSkippedTestResult(3, testRunResult, 3, [2, 1]);
    }

    [Fact]
    public void AddTestCase_TestingOperationIsBroken_TestCaseHasException()
    {
        // Arrange
        var builder = TestSuite.Sequential.WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.AdderThrowsCustomException);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        
        var validatedTestCase = testRunResult.ValidatedTestCases
            .FirstOrDefault(x => x.TestCase.Number == 1);
        Assert.NotNull(validatedTestCase);
        Assert.Equal(AssertStatus.NotPassedWithException, validatedTestCase.AssertStatus);
        Assert.Equal(ValidationStatus.Valid, validatedTestCase.ValidationStatus);
        var testCase = validatedTestCase.TestCase;
        Assert.NotNull(testCase.Assert.Value.Exception);
        Assert.IsType<CustomException>(testCase.Assert.Value.Exception);
    }
    
    [Fact]
    public void Run_InvalidIterationNumber_ThrowsException()
    {
        // Arrange
        var builder1 = SetupAdderBuilder();
        var builder2 = SetupAdderBuilder();
        
        // Act
        var reporter1 = builder1.Expect(2).WithInput(1, 1).Run(2);
        var reporter2 = builder2.Expect(2).WithInput(1, 1).Run(1, 2);
        
        // Assert
        var testRunResult1 = TestHelpers.GetTestRunResultFromReporter(reporter1);
        AssertContextValidationHasFailed(testRunResult1, ValidationSubject.TestNumbers);
        var testRunResult2 = TestHelpers.GetTestRunResultFromReporter(reporter2);
        AssertContextValidationHasFailed(testRunResult2, ValidationSubject.TestNumbers);
    }
    
    [Fact]
    public void TestRunBuilder_InvalidDelegateReturnType_ShouldThrow()
    {
        // Arrange
        var context = TestHelpers.CreateEmptyContext<int>();
        var builder = new TestRunBuilder<int>(context);
        
        // Act
        var reporter = builder.UseOperation((int _, int _) => "test").Expect(2).WithInput(1, 1).Run();
        
        // Assert
        var testRunResult1 = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertContextValidationHasFailed(testRunResult1, ValidationSubject.Operation);
    }
    
    [Fact]
    public void AddTestCase_TestingOperationNotSet_AdderWithAttributeShouldBeSelected()
    {
        // Arrange
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock
            .Setup(x => x.Get())
            .Returns(Assembly.GetAssembly(typeof(TestRunBuilderTests)));
        var context = TestHelpers.CreateEmptyContext<int>(entryAssemblyProviderMock.Object);
        var builder = new TestRunBuilder<int>(context);
        
        // Act
        var reporter = builder
            .Expect(2).WithInput(1, 1)
            .Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertPassedTestResult(1, testRunResult, 2, [1, 1]);
    }
    
    [Fact]
    public void AddTestCase_WithCustomEquatableObject_ValidReturn()
    {
        // Arrange
        var builder = TestSuite.Sequential
            .WithExpectedReturnType<EquatableTestObject>()
            .UseOperation((EquatableTestObject a, EquatableTestObject b) => new EquatableTestObject(a.Value + b.Value));
        
        // Act
        var reporter = builder
            .Expect(new EquatableTestObject(2)).WithInput(new EquatableTestObject(1), new EquatableTestObject(1))
            .Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertPassedTestResult(1, testRunResult, new EquatableTestObject(2), [new EquatableTestObject(1), new EquatableTestObject(1)]);
    }
    
    [Fact]
    public void UseOperation_WithCustomObjectAndComparer_ValidReturn()
    {
        // Arrange
        var comparer = (TestObject a, TestObject b) => a.Value == b.Value;
        var setup = TestSuite.Sequential
            .WithExpectedReturnType(comparer);
        
        // Act
        var reporter = setup
            .UseOperation((TestObject a, TestObject b) => new TestObject(a.Value + b.Value))
            .Expect(new TestObject(2)).WithInput([new TestObject(1), new TestObject(1)])
            .Run();

        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertPassedTestResult(1, testRunResult, new TestObject(2), [new TestObject(1), new TestObject(1)], comparer);
    }
    
    [Fact]
    public void UseOperation_WithCustomObject_ShouldThrow()
    {
        // Arrange
        
        // Act
        var func = () => TestSuite.Sequential.WithExpectedReturnType<TestObject>().Run();

        // Assert
        Assert.Throws<InvalidOperationException>(func);
    }
    
    [Fact]
    public void TestSuite_Ignored_TestCasesShouldBeIgnored()
    {
        // Arrange
        var builder = TestSuite.Sequential.Ignore.WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder)
            .Expect(2).WithInput(1, 1, 1)
            .Expect(3).WithInput(1, 1, 1);
            
        // Act    
        var reporter = builder.Run();
        
        // Assert
        var testRunResult = TestHelpers.GetTestRunResultFromReporter(reporter);
        AssertSkippedTestResult(1, testRunResult, 2, [1, 1, 1]);
        AssertSkippedTestResult(2, testRunResult, 3, [1, 1, 1]);
    }

    private static void AssertPassedTestResult<TOutput>(
        int testNumber,
        TestRunResult<TOutput> testRunResult, 
        TOutput expected, 
        object[] inputs,
        Func<TOutput?, TOutput?, bool>? comparer = null)
    {
        var validatedTestCase = testRunResult.ValidatedTestCases
            .FirstOrDefault(x => x.TestCase.Number == testNumber);
        
        Assert.NotNull(validatedTestCase);
        Assert.Equal(AssertStatus.Passed, validatedTestCase.AssertStatus);
        Assert.Equal(ValidationStatus.Valid, validatedTestCase.ValidationStatus);
        Assert.NotEmpty(validatedTestCase.ValidationResults);
        foreach (var validationResult in validatedTestCase.ValidationResults)
            Assert.True(validationResult.IsValid);

        var testCase = validatedTestCase.TestCase;
        Assert.True(testCase.Assert.IsValueCreated);
        Assert.NotNull(testCase.Assert.Value.Output);
        if (typeof(TOutput) == typeof(object) || typeof(IEquatable<TOutput>).IsAssignableFrom(typeof(TOutput)))
        {
            Assert.Equal(expected, testCase.Expected);
            Assert.Equal(inputs, testCase.Inputs);
            Assert.Equal(testCase.Expected, testCase.Assert.Value.Output.Value);
        }
        else
        {
            Assert.True(comparer?.Invoke(expected, testCase.Expected));
            Assert.True(comparer?.Invoke(testCase.Expected, testCase.Assert.Value.Output.Value));
            foreach (var input in inputs.Zip(testCase.Inputs))
                Assert.True(comparer?.Invoke((TOutput?)input.First, (TOutput?)input.Second));
        }
        Assert.True(testCase.Assert.Value.Passed);
        Assert.Null(testCase.Assert.Value.Exception);
        Assert.True(testCase.Assert.Value.ElapsedTime.TotalMilliseconds > 0);
    }
    
    private static void AssertNotPassedTestResult<TOutput>(
        int testNumber,
        TestRunResult<TOutput> testRunResult, 
        TOutput expected, 
        object[] inputs)
    {
        var validatedTestCase = testRunResult.ValidatedTestCases
            .FirstOrDefault(x => x.TestCase.Number == testNumber);
        
        Assert.NotNull(validatedTestCase);
        Assert.Equal(AssertStatus.NotPassed, validatedTestCase.AssertStatus);
        Assert.Equal(ValidationStatus.Valid, validatedTestCase.ValidationStatus);
        Assert.NotEmpty(validatedTestCase.ValidationResults);
        foreach (var validationResult in validatedTestCase.ValidationResults)
            Assert.True(validationResult.IsValid);
        
        var testCase = validatedTestCase.TestCase;
        Assert.True(testCase.Assert.IsValueCreated);
        Assert.Equal(expected, testCase.Expected);
        Assert.Equal(inputs, testCase.Inputs);
        Assert.True(testCase.Assert.IsValueCreated);
        Assert.False(testCase.Assert.Value.Passed);
        Assert.Null(testCase.Assert.Value.Exception);
        Assert.NotNull(testCase.Assert.Value.Output);
        Assert.NotEqual(testCase.Expected, testCase.Assert.Value.Output.Value);
        Assert.True(testCase.Assert.Value.ElapsedTime.TotalMilliseconds > 0);
    }

    private void AssertSkippedTestResult<TOutput>(
        int testNumber,
        TestRunResult<TOutput> testRunResult,
        TOutput expected, 
        object[] inputs)
    {
        var validatedTestCase = testRunResult.ValidatedTestCases
            .FirstOrDefault(x => x.TestCase.Number == testNumber);
        
        Assert.NotNull(validatedTestCase);
        Assert.Equal(AssertStatus.Unknown, validatedTestCase.AssertStatus);
        Assert.Equal(ValidationStatus.Unknown, validatedTestCase.ValidationStatus);
        
        var testCase = validatedTestCase.TestCase;
        Assert.False(testCase.Assert.IsValueCreated);
        Assert.Equal(expected, testCase.Expected);
        Assert.Equal(inputs, testCase.Inputs);
    }

    private static TestRunBuilder<int> SetupAdderBuilder()
    {
        return TestSuite.Sequential.WithExpectedReturnType<int>()
            .UseOperation(StaticMethods.Adder);
    }
    
    [TestSuiteDelegate]
    private static int AdderWithAttribute(int number1, int number2)
    {
        return number1 + number2;
    }
    
    private class EquatableTestObject(int value) : IEquatable<EquatableTestObject>
    {
        public int Value { get; } = value;

        public bool Equals(EquatableTestObject? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EquatableTestObject)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
    
    private class TestObject(int value)
    {
        public int Value { get; } = value;
    }
}