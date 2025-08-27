using System.Reflection;
using Moq;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestCase.Clause;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.UnitTests.Helpers.Extensions;
using SimpleFluentTester.UnitTests.Helpers.TestObjects;
using SimpleFluentTester.Validators.Models;

namespace SimpleFluentTester.UnitTests.Tests.TestSuiteBuilder;

public sealed class ExpectParameterTests
{
    [Fact]
    public void ExpectParameter_OnlyParameterWithValidCalculations_ShouldBeValid()
    {
        var builder = PrepareBuilder();
        builder.UseOperation((int[] seq1, int[] seq2) =>
        {
            for (var i = 0; i < seq1.Length; i++)
                seq1[i] += seq2[i];
        });
        
        // Act
        var reporter = builder
            .ExpectParameter(0).ToBe(new[] {4, 6}).WithInput(new[] {1, 2}, new[] {3, 4})
            .ExpectParameter(1).ToBe(new[] {3, 4}).WithInput(new[] {1, 2}, new[] {3, 4})
            .Run();
        
        // Assert
#pragma warning disable CS8604 // Possible null reference argument.
        Func<int[]?, int[]?, bool> comparer = (x, y) => x.SequenceEqual(y);
#pragma warning restore CS8604 // Possible null reference argument.
        reporter.AssertTestCaseExists(1).AssertPassed([4, 6], [new[] {1, 2}, new[] {3, 4}], comparer);
        reporter.AssertTestCaseExists(2).AssertPassed([3, 4], [new[] {1, 2}, new[] {3, 4}], comparer);
    }
    
    [Fact]
    public void ExpectParameter_OnlyParameterWithNotValidCalculations_ShouldBeNotValid()
    {
        var builder = PrepareBuilder();
        builder.UseOperation((int[] seq1, int[] seq2) =>
        {
            for (var i = 0; i < seq1.Length; i++)
                seq1[i] += seq2[i];
        });
        
        // Act
        var reporter = builder
            .ExpectParameter(0).ToBe(new[] {1, 2}).WithInput(new[] {1, 2}, new[] {3, 4})
            .Run();
        
        // Assert
#pragma warning disable CS8604 // Possible null reference argument.
        Func<int[]?, int[]?, bool> comparer = (x, y) => x.SequenceEqual(y);
#pragma warning restore CS8604 // Possible null reference argument.
        reporter.AssertTestCaseExists(1).AssertNotPassed([4, 6], [new[] {1, 2}, new[] {3, 4}], comparer);
    }
    
    [Fact]
    public void ExpectParameter_OnlyParameterWithOperationException_ShouldBeFailed()
    {
        var builder = PrepareBuilder();
        Func<int[], int[], int> operation = (_, _) => throw new CustomException();
        builder.UseOperation(operation);
        
        // Act
        var reporter = builder
            .ExpectParameter(0).ToBe(new[] {1, 2}).WithInput(new[] {1, 2}, new[] {3, 4})
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertNotPassedWithException(new[] {1, 2}, [new[] {1, 2}, new[] {3, 4}], typeof(CustomException));
    }
    
    [Fact]
    public void ExpectParameter_OnlyParameterWithComparerException_ShouldBeFailed()
    {
        var builder = PrepareBuilder();
        builder.UseOperation((int[] seq1, int[] seq2) =>
        {
            for (var i = 0; i < seq1.Length; i++)
                seq1[i] += seq2[i];
        });
        const string innerMessage = "text";
        builder.WithComparer((int[]? _, int[]? _) => throw new CustomWithMessageException(innerMessage));
        
        // Act
        var reporter = builder
            .ExpectParameter(0).ToBe(new[] {1, 2}).WithInput(new[] {1, 2}, new[] {3, 4})
            .Run();
        
        // Assert
        reporter.AssertTestCaseExists(1).AssertFailedValidation<CustomWithMessageException>(ValidationSubject.Comparer, "Comparer execution failed with an exception.", innerMessage);
    }
    
    private SequentialTestSuiteBuilder PrepareBuilder()
    {
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock
            .Setup(x => x.Get())
            .Returns(Assembly.GetAssembly(typeof(ExpectTests)));
        var container = TestSuiteContextContainer.Default();
        container.WithEntryAssemblyProvider(entryAssemblyProviderMock.Object);
        return new SequentialTestSuiteBuilder(container, new List<DefinedTestClause>());
    }
}