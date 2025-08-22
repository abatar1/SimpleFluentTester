using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleFluentTester.TestSuite.Context;

/// <summary>
/// Represents a utility class responsible for enriching operations within a test suite context.
/// </summary>
internal class OperationEnricher(ITestSuiteContextContainer container)
{
    private static Delegate? _operationDelegate;

    /// <summary>
    /// Attempts to enrich the test operation attribute for the context container by invoking the core enrichment logic.
    /// </summary>
    public void TryToEnrichAttributeOperation()
    {
        TryToEnrichAttributeOperationCore(_operationDelegate);
    }

    /// <summary>
    /// For testing purposes only to avoid failures with multiple test units.
    /// </summary>
    internal void TryToEnrichAttributeOperation(Delegate? operationDelegate)
    {
        TryToEnrichAttributeOperationCore(operationDelegate);
    }

    /// <summary>
    /// Attempts to enrich the attribute operation by resolving or assigning a delegate within the test suite context container.
    /// </summary>
    /// <param name="operationDelegate">
    /// An optional delegate to be utilized for enriching the operation within the context.
    /// If null, the delegate is resolved dynamically from the attributed method.
    /// </param>
    private void TryToEnrichAttributeOperationCore(Delegate? operationDelegate)
    {
        if (container.Context.Operation != null)
            return;
        _operationDelegate = GetDelegateFromAttributedMethod(operationDelegate);
        container.WithOperation(_operationDelegate);
    }

    /// <summary>
    /// Retrieves a delegate based on the method annotated in the test suite context, resolving it dynamically from the container and assembly metadata.
    /// </summary>
    /// <param name="operationDelegate">An optional delegate that, if provided, will be returned directly without further resolution.</param>
    /// <returns>A delegate instance derived from the attributed method in the test suite context or the provided delegate if not null.</returns>
    /// <exception cref="InvalidContextException">
    /// Thrown when the attributed method's declaring type cannot be resolved or when an error occurs during instance creation.
    /// </exception>
    private Delegate GetDelegateFromAttributedMethod(Delegate? operationDelegate)
    {
        if (operationDelegate != null)
            return operationDelegate;

        var assemblyMethodOfTestSuite = TryGetAssemblyMethodOfTestSuite(container);

        var delegateType = GetDelegateType(assemblyMethodOfTestSuite);

        if (assemblyMethodOfTestSuite.IsStatic)
            return assemblyMethodOfTestSuite.CreateDelegate(delegateType);

        var methodClassType = assemblyMethodOfTestSuite.DeclaringType;
        
        if (methodClassType == null)
            throw new InvalidContextException("Method type is null, something went wrong");

        ValidateMethodClassType(methodClassType);

        var target = container.Context.Activator.CreateInstance(methodClassType);

        return assemblyMethodOfTestSuite.CreateDelegate(delegateType, target);
    }

    /// <summary>
    /// Attempts to retrieve a single method marked with the <see cref="TestSuiteDelegateAttribute"/> within the entry assembly of the provided test suite context container.
    /// </summary>
    /// <param name="container">
    /// The container holding the test suite context, which provides access to the entry assembly.
    /// </param>
    /// <returns>
    /// The <see cref="MethodInfo"/> of the method marked with the <see cref="TestSuiteDelegateAttribute"/> in the entry assembly.
    /// </returns>
    /// <exception cref="InvalidContextException">
    /// Thrown if no method is marked with the attribute, if the entry assembly is null, or if more than one method is marked with the attribute.
    /// </exception>
    private static MethodInfo TryGetAssemblyMethodOfTestSuite(ITestSuiteContextContainer container)
    {
        var entryAssembly = container.Context.EntryAssemblyProvider.Get();
        if (entryAssembly == null)
            throw new InvalidContextException($"No entry {nameof(Assembly)} have been found when trying to find {nameof(TestSuiteDelegateAttribute)} definitions.");

        const BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        var operationMembers = entryAssembly.GetTypes()
            .SelectMany(type => type.GetMembers(bindingAttr))
            .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(TestSuiteDelegateAttribute)))
            .ToList();

        switch (operationMembers.Count)
        {
            case 0:
                throw new InvalidContextException($"You should specify an operation first with an {nameof(TestSuiteDelegateAttribute)} attribute or using {nameof(SequentialTestSuiteBuilder.UseOperation)} method.");
            case > 1:
                throw new InvalidContextException($"You defined more than one method with {nameof(TestSuiteDelegateAttribute)}.");
        }

        return (MethodInfo)operationMembers.Single();
    }

    /// <summary>
    /// Constructs a delegate type based on the method signature provided by the given <see cref="MethodInfo"/> instance.
    /// </summary>
    /// <param name="assemblyMethodOfTestSuite">The method information containing parameters and return type used to construct the delegate type.</param>
    /// <returns>The constructed delegate type reflecting the method's signature.</returns>
    private static Type GetDelegateType(MethodInfo assemblyMethodOfTestSuite)
    {
        var assemblyMethodParameterOfTestSuite = assemblyMethodOfTestSuite.GetParameters()
            .Select(x => x.ParameterType)
            .Append(assemblyMethodOfTestSuite.ReturnType)
            .ToArray();
        return Expression.GetDelegateType(assemblyMethodParameterOfTestSuite);
    }

    /// <summary>
    /// Validates the class type of a method's declaring type to ensure it adheres to the required constraints.
    /// Specifically, it checks if the class has a public parameterless constructor, which is necessary
    /// when using non-static methods in conjunction with the <see cref="TestSuiteDelegateAttribute"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the method's declaring class to be validated.</param>
    /// <exception cref="InvalidContextException">
    /// Thrown when the provided type does not have a public parameterless constructor,
    /// which is required when applying the <see cref="TestSuiteDelegateAttribute"/> to non-static methods.
    /// </exception>
    private static void ValidateMethodClassType(Type type)
    {
        var methodClassCtor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        var assemblyMethodClassCtorOfTestSuite = methodClassCtor
            .FirstOrDefault(x => x.GetParameters().Length == 0);
        if (assemblyMethodClassCtorOfTestSuite == null)
            throw new InvalidContextException($"{nameof(TestSuiteDelegateAttribute)} has been defined for non-static method where declaring type do not have empty constructors. Please add empty constructor or consider using static method.");
    }
}