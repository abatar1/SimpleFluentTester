using System.Reflection;
using Moq;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.UnitTests.Tests;

public class OperationValidatorOperatorNotSetTests
{
    [Fact]
    public void GetDelegateFromAttributedMethod_AssemblyIsNull_ShouldThrow()
    {
        // Assign
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        var context = TestHelpers.CreateEmptyContext<object>(entryAssemblyProviderMock.Object);
        var operationValidator = new OperationValidator();
        var operationValidatedObject = new OperationValidatedObject(typeof(object));
        
        // Act 
        var validationResult = operationValidator.Validate(context, operationValidatedObject);

        // Assert
        var message = $"No entry Assembly have been found when trying to find {nameof(TestSuiteDelegateAttribute)} definitions";
        AssertNotValidValidation(validationResult, message);
    }
    
    [Fact]
    public void GetDelegateFromAttributedMethod_AssemblyDoNotHaveTypes_ShouldThrow()
    {
        // Assign
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetTypes()).Returns(Array.Empty<Type>());
        
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock.Setup(x => x.Get()).Returns(assemblyMock.Object);
        
        var context = TestHelpers.CreateEmptyContext<object>(entryAssemblyProviderMock.Object);
        var operationValidator = new OperationValidator();
        var operationValidatedObject = new OperationValidatedObject(typeof(object));
        
        // Act 
        var validationResult = operationValidator.Validate(context, operationValidatedObject);

        // Assert
        var message =
            $"You should specify an operation first with an {nameof(TestSuiteDelegateAttribute)} attribute or using {nameof(TestSuiteBuilder<object>.UseOperation)} method";
        AssertNotValidValidation(validationResult, message);
    }
    
    [Fact]
    public void GetDelegateFromAttributedMethod_AssemblyHasMoreThanOneDefinedMethod_ShouldThrow()
    {
        // Assign
        var memberInfos = new List<MemberInfo>();
        for (var i = 0; i < 3; i++)
            memberInfos.Add(GetMockedMethodWithAttribute().Object);
        
        var classTypeMock = GetParentClassTypeMock(memberInfos.ToArray());
        
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetTypes()).Returns([classTypeMock.Object]);
        
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock.Setup(x => x.Get()).Returns(assemblyMock.Object);
        
        var context = TestHelpers.CreateEmptyContext<object>(entryAssemblyProviderMock.Object);
        var operationValidator = new OperationValidator();
        var operationValidatedObject = new OperationValidatedObject(typeof(object));
        
        // Act 
        var validationResult = operationValidator.Validate(context, operationValidatedObject);

        // Assert
        var message = $"You defined more than one method with {nameof(TestSuiteDelegateAttribute)}";
        AssertNotValidValidation(validationResult, message);
    }
    
    [Fact]
    public void GetDelegateFromAttributedMethod_StaticMethodWithSingleAttribute_ShouldBeValid()
    {
        // Assign
        var parameterTypeMock = new Mock<Type>();
        
        var parameterInfoMock = new Mock<ParameterInfo>();
        parameterInfoMock.SetupGet(x => x.ParameterType).Returns(parameterTypeMock.Object);
        
        var memberInfoMock = GetMockedMethodWithAttribute();
        Delegate expectedDelegate = () => 1;
        memberInfoMock
            .Setup(x => x.CreateDelegate(It.IsAny<Type>()))
            .Returns(expectedDelegate);
        memberInfoMock.SetupGet(x => x.ReturnType).Returns(typeof(int));
        memberInfoMock.Setup(x => x.GetParameters()).Returns([parameterInfoMock.Object]);
        memberInfoMock.SetupGet(x => x.Attributes).Returns(MethodAttributes.Static);
            
        var classTypeMock = GetParentClassTypeMock(memberInfoMock.Object);
        
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetTypes()).Returns([classTypeMock.Object]);
        
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock.Setup(x => x.Get()).Returns(assemblyMock.Object);
        
        var context = TestHelpers.CreateEmptyContext<int>(entryAssemblyProviderMock.Object);
        var operationValidator = new OperationValidator();
        var operationValidatedObject = new OperationValidatedObject(typeof(int));
        
        // Act 
        var validationResult = operationValidator.Validate(context, operationValidatedObject);

        // Assert
        AssertValidValidation(validationResult, context, expectedDelegate);
    }
    
    [Fact]
    public void GetDelegateFromAttributedMethod_NonStaticMethodDefinedInClassWithoutEmptyConstructor_ShouldThrow()
    {
        // Assign
        var returnType = new Mock<Type>();
        var parameterTypeMock = new Mock<Type>();
        
        var parameterInfoMock = new Mock<ParameterInfo>();
        parameterInfoMock.SetupGet(x => x.ParameterType).Returns(parameterTypeMock.Object);
        
        var memberInfoMock = GetMockedMethodWithAttribute();
        memberInfoMock.SetupGet(x => x.ReturnType).Returns(returnType.Object);
        memberInfoMock.Setup(x => x.GetParameters()).Returns([parameterInfoMock.Object]);

        var classTypeMock = GetParentClassTypeMock(memberInfoMock.Object);
        var constructorMock = new Mock<ConstructorInfo>();
        constructorMock
            .Setup(x => x.GetParameters())
            .Returns(Enumerable.Repeat(Mock.Of<ParameterInfo>(), 3).ToArray());
        classTypeMock
            .Setup(x => x.GetConstructors(It.Is<BindingFlags>(y => y == (BindingFlags.Public | BindingFlags.Instance))))
            .Returns([constructorMock.Object]);
        memberInfoMock.SetupGet(x => x.DeclaringType).Returns(classTypeMock.Object);
        
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetTypes()).Returns([classTypeMock.Object]);
        
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock.Setup(x => x.Get()).Returns(assemblyMock.Object);
        
        var context = TestHelpers.CreateEmptyContext<object>(entryAssemblyProviderMock.Object);
        var operationValidator = new OperationValidator();
        var operationValidatedObject = new OperationValidatedObject(typeof(object));
        
        // Act 
        var validationResult = operationValidator.Validate(context, operationValidatedObject);

        // Assert
        var message =
            $"{nameof(TestSuiteDelegateAttribute)} has been defined for non-static method where declaring type do not have empty constructors, please add empty constructor or consider using static method.";
        AssertNotValidValidation(validationResult, message);
    }
    
    [Fact]
    public void GetDelegateFromAttributedMethod_NonStaticMethodDefinedInClassWithEmptyConstructor_ShouldBeValid()
    {
        // Assign
        var returnType = new Mock<Type>();
        var parameterTypeMock = new Mock<Type>();
        
        var parameterInfoMock = new Mock<ParameterInfo>();
        parameterInfoMock.SetupGet(x => x.ParameterType).Returns(parameterTypeMock.Object);
        
        var memberInfoMock = GetMockedMethodWithAttribute();
        memberInfoMock.SetupGet(x => x.ReturnType).Returns(returnType.Object);
        memberInfoMock.Setup(x => x.GetParameters()).Returns([parameterInfoMock.Object]);

        var declaringTypeMock = GetParentClassTypeMock(memberInfoMock.Object);
        var constructorMock = new Mock<ConstructorInfo>();
        constructorMock
            .Setup(x => x.GetParameters())
            .Returns(Array.Empty<ParameterInfo>());
        declaringTypeMock
            .Setup(x => x.GetConstructors(It.Is<BindingFlags>(y => y == (BindingFlags.Public | BindingFlags.Instance))))
            .Returns([constructorMock.Object]);
        memberInfoMock.SetupGet(x => x.DeclaringType).Returns(declaringTypeMock.Object);
        
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetTypes()).Returns([declaringTypeMock.Object]);
        
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock.Setup(x => x.Get()).Returns(assemblyMock.Object);

        var declaringTypeObjectMock = new Mock<object>();
        var activatorMock = new Mock<IActivator>();
        activatorMock
            .Setup(x => x.CreateInstance(It.Is<Type>(y => y == declaringTypeMock.Object)))
            .Returns(declaringTypeObjectMock.Object);
        
        Delegate expectedDelegate = () => 1;
        memberInfoMock
            .Setup(x => x.CreateDelegate(It.IsAny<Type>(), It.Is<object>(y => y == declaringTypeObjectMock.Object)))
            .Returns(expectedDelegate);
        
        var context = TestHelpers.CreateEmptyContext<int>(entryAssemblyProviderMock.Object, activatorMock.Object);
        var operationValidator = new OperationValidator();
        var operationValidatedObject = new OperationValidatedObject(typeof(int));
        
        // Act 
        var validationResult = operationValidator.Validate(context, operationValidatedObject);

        // Assert
        AssertValidValidation(validationResult, context, expectedDelegate);
    }
    
    private static void AssertNotValidValidation(ValidationResult validationResult, string message)
    {
        Assert.False(validationResult.IsValid);
        Assert.Equal(ValidationSubject.Operation, validationResult.ValidationSubject);
        Assert.Equal(message, validationResult.Message);
    }
    
    private static void AssertValidValidation<TOutput>(
        ValidationResult validationResult,
        ITestSuiteBuilderContext<TOutput> context,
        Delegate expectedDelegate)
    {
        Assert.True(validationResult.IsValid);
        Assert.Equal(expectedDelegate, context.Operation.Value);
        Assert.Equal(ValidationSubject.Operation, validationResult.ValidationSubject);
        Assert.Null(validationResult.Message);
    }

    private static Mock<Type> GetParentClassTypeMock(params MemberInfo[] methodInfos)
    {
        var classTypeMock = new Mock<Type>();
        classTypeMock
            .Setup(x => x.GetMembers(It.Is<BindingFlags>(y =>
                y == (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))))
            .Returns(methodInfos);
        return classTypeMock;
    }

    private static Mock<MethodInfo> GetMockedMethodWithAttribute()
    {
        var memberInfoMock = new Mock<MethodInfo>();
        var customAttributeDateMock = new Mock<CustomAttributeData>();
        customAttributeDateMock
            .SetupGet(x => x.AttributeType)
            .Returns(typeof(TestSuiteDelegateAttribute));
        memberInfoMock
            .SetupGet(x => x.CustomAttributes)
            .Returns([customAttributeDateMock.Object]);
        return memberInfoMock;
    }
}