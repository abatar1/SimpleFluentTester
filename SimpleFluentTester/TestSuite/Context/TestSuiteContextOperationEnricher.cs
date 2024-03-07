using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Context;

public static class OperationEnricher
{
    public static ITestSuiteBuilderContext<TOutput> TryToEnrichAttributeOperation<TOutput>(this ITestSuiteBuilderContext<TOutput> context)
    {
        if (context.Operation != null)
            return context;

        try
        {
            var operation = GetDelegateFromAttributedMethod(context.EntryAssemblyProvider, context.Activator);
            context.AddValidation(ValidationResult.Ok(ValidationSubject.Operation));
            return context.WithOperation(operation);
        }
        catch (Exception e)
        {
            context.AddValidation(ValidationResult.Failed(ValidationSubject.Operation, e.Message));
            return context;
        }
    }

    private static Delegate GetDelegateFromAttributedMethod(IEntryAssemblyProvider entryAssemblyProvider,
        IActivator activator)
    {
        var entryAssembly = entryAssemblyProvider.Get();
        if (entryAssembly == null)
            throw new InvalidOperationException(
                $"No entry {nameof(Assembly)} have been found when trying to find {nameof(TestSuiteDelegateAttribute)} definitions.");

        const BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        var operationMembers = entryAssembly.GetTypes()
            .SelectMany(type => type.GetMembers(bindingAttr))
            .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(TestSuiteDelegateAttribute)))
            .ToList();

        if (operationMembers.Count == 0)
            throw new InvalidOperationException(
                $"You should specify an operation first with an {nameof(TestSuiteDelegateAttribute)} attribute or using {nameof(TestSuiteBuilder<object>.UseOperation)} method.");
        if (operationMembers.Count > 1)
            throw new InvalidOperationException(
                $"You defined more than one method with {nameof(TestSuiteDelegateAttribute)}.");

        var assemblyMethodOfTestSuite = (MethodInfo)operationMembers.Single();

        var assemblyMethodParameterOfTestSuite = assemblyMethodOfTestSuite.GetParameters()
            .Select(x => x.ParameterType)
            .Append(assemblyMethodOfTestSuite.ReturnType)
            .ToArray();
        var delegateType = Expression.GetDelegateType(assemblyMethodParameterOfTestSuite);

        if (assemblyMethodOfTestSuite.IsStatic)
            return assemblyMethodOfTestSuite.CreateDelegate(delegateType);

        var methodClassType = assemblyMethodOfTestSuite.DeclaringType;
        var methodClassCtor = methodClassType!.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        var assemblyMethodClassCtorOfTestSuite = methodClassCtor
            .FirstOrDefault(x => x.GetParameters().Length == 0);
        if (assemblyMethodClassCtorOfTestSuite == null)
            throw new InvalidOperationException(
                $"{nameof(TestSuiteDelegateAttribute)} has been defined for non-static method where declaring type do not have empty constructors. Please add empty constructor or consider using static method.");

        var target = activator.CreateInstance(methodClassType);

        return assemblyMethodOfTestSuite.CreateDelegate(delegateType, target);
    }
}