using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimpleFluentTester.Helpers;

namespace SimpleFluentTester.TestSuite;

internal static class TestSuiteDelegateHelper
{
    public static Delegate GetDelegateFromAttributedMethod(IEntryAssemblyProvider entryAssemblyProvider, IActivator activator)
    {
        var entryAssembly = entryAssemblyProvider.Get();
        if (entryAssembly == null)
            throw new InvalidOperationException("No entry Assembly have been found when trying to find TestSuiteDelegateAttribute definitions");
            
        var operationMembers = entryAssembly.GetTypes()
            .SelectMany(type => type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(TestSuiteDelegateAttribute)))
            .ToList();
            
        if (operationMembers.Count == 0)
            throw new InvalidOperationException($"You should specify an operation first with an {nameof(TestSuiteDelegateAttribute)} attribute or using UseOperation method");
        if (operationMembers.Count > 1)
            throw new InvalidOperationException($"You defined more than one method with {nameof(TestSuiteDelegateAttribute)}");
            
        var assemblyMethodOfTestSuite = (MethodInfo)operationMembers.Single();

        var assemblyMethodParameterOfTestSuite = assemblyMethodOfTestSuite.GetParameters()
            .Select(x => x.ParameterType)
            .Append(assemblyMethodOfTestSuite.ReturnType)
            .ToArray();
        var delegateType = Expression.GetDelegateType(assemblyMethodParameterOfTestSuite);

        if (assemblyMethodOfTestSuite.IsStatic)
            return assemblyMethodOfTestSuite.CreateDelegate(delegateType);

        var methodClassType = assemblyMethodOfTestSuite.DeclaringType;
        if (methodClassType == null)
            throw new InvalidOperationException("No declaring type for non-static method, should be the bug");

        var methodClassCtor = methodClassType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        var assemblyMethodClassCtorOfTestSuite = methodClassCtor
            .FirstOrDefault(x => x.GetParameters().Length == 0);
        if (assemblyMethodClassCtorOfTestSuite == null)
            throw new InvalidOperationException("TestSuiteDelegateAttribute has been defined for non-static method where declaring type do not have empty constructors, please add empty constructor or consider using static method.");

        var target = activator.CreateInstance(methodClassType);
        
        return assemblyMethodOfTestSuite.CreateDelegate(delegateType, target);
    }
}