using System.Reflection;
using Moq;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.UnitTests;

public class TestSuiteDelegateHelperTests
{
    [Fact]
    public void GetDelegateFromAttributedMethod_AssemblyIsNull_ShouldThrow()
    {
        // Assign
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        
        // Act 
        var func = () => TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(entryAssemblyProviderMock.Object, new DefaultActivator());

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(func);
        Assert.Equal("No entry Assembly have been found when trying to find TestSuiteDelegateAttribute definitions", exception.Message);
    }
    
    [Fact]
    public void GetDelegateFromAttributedMethod_AssemblyDoNotHaveTypes_ShouldThrow()
    {
        // Assign
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetTypes()).Returns(Array.Empty<Type>());
        
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock.Setup(x => x.Get()).Returns(assemblyMock.Object);
        
        // Act 
        var func = () => TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(entryAssemblyProviderMock.Object, new DefaultActivator());

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(func);
        Assert.Equal($"You should specify an operation first with an {nameof(TestSuiteDelegateAttribute)} attribute or using UseOperation method", exception.Message);
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
        
        // Act 
        var func = () => TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(entryAssemblyProviderMock.Object, new DefaultActivator());

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(func);
        Assert.Equal($"You defined more than one method with {nameof(TestSuiteDelegateAttribute)}", exception.Message);
    }
    
    [Fact]
    public void GetDelegateFromAttributedMethod_StaticMethodWithSingleAttribute_ShouldBeValid()
    {
        // Assign
        var returnType = new Mock<Type>();
        var parameterTypeMock = new Mock<Type>();
        
        var parameterInfoMock = new Mock<ParameterInfo>();
        parameterInfoMock.SetupGet(x => x.ParameterType).Returns(parameterTypeMock.Object);
        
        var memberInfoMock = GetMockedMethodWithAttribute();
        Delegate expectedDelegate = () => { };
        memberInfoMock
            .Setup(x => x.CreateDelegate(It.IsAny<Type>()))
            .Returns(expectedDelegate);
        memberInfoMock.SetupGet(x => x.ReturnType).Returns(returnType.Object);
        memberInfoMock.Setup(x => x.GetParameters()).Returns([parameterInfoMock.Object]);
        memberInfoMock.SetupGet(x => x.Attributes).Returns(MethodAttributes.Static);
            
        var classTypeMock = GetParentClassTypeMock(memberInfoMock.Object);
        
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetTypes()).Returns([classTypeMock.Object]);
        
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock.Setup(x => x.Get()).Returns(assemblyMock.Object);
        
        // Act 
        var resultDelegate = TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(entryAssemblyProviderMock.Object, new DefaultActivator());

        // Assert
        Assert.Equal(expectedDelegate, resultDelegate);
    }
    
    [Fact]
    public void GetDelegateFromAttributedMethod_NoDeclaringTypeForNonStaticMethod_ShouldThrow()
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
        
        var assemblyMock = new Mock<Assembly>();
        assemblyMock.Setup(x => x.GetTypes()).Returns([classTypeMock.Object]);
        
        var entryAssemblyProviderMock = new Mock<IEntryAssemblyProvider>();
        entryAssemblyProviderMock.Setup(x => x.Get()).Returns(assemblyMock.Object);
        
        // Act 
        var func = () => TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(entryAssemblyProviderMock.Object, new DefaultActivator());

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(func);
        Assert.Equal("No declaring type for non-static method, should be the bug", exception.Message);
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
        
        // Act 
        var func = () => TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(entryAssemblyProviderMock.Object, new DefaultActivator());

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(func);
        Assert.Equal("TestSuiteDelegateAttribute has been defined for non-static method where declaring type do not have empty constructors, please add empty constructor or consider using static method.", exception.Message);
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
        
        Delegate expectedDelegate = () => { };
        memberInfoMock
            .Setup(x => x.CreateDelegate(It.IsAny<Type>(), It.Is<object>(y => y == declaringTypeObjectMock.Object)))
            .Returns(expectedDelegate);
        
        // Act 
        var resultDelegate = TestSuiteDelegateHelper.GetDelegateFromAttributedMethod(entryAssemblyProviderMock.Object, activatorMock.Object);

        // Assert
        Assert.Equal(expectedDelegate, resultDelegate);
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