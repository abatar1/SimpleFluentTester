using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimpleFluentTester.Helpers;
using SimpleFluentTester.TestSuite;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class OperationValidator : BaseValidator
{
    public override ValidationResult Validate<TOutput>(
        ITestSuiteBuilderContext<TOutput> context, 
        IValidatedObject validatedObject)
    {
        if (validatedObject is not OperationValidatedObject nonTypedOperationValidatedObject)
            throw new ValidationUnexpectedException("Was not able to cast validated object to it's type, seems like a bug.");

        if (context.Operation.Value == null)
        {
            try
            {
                context.Operation.Value = GetDelegateFromAttributedMethod(context.EntryAssemblyProvider, context.Activator);
            }
            catch (Exception e)
            {
                return ValidationResult.Failed(ValidationSubject.Operation, e.Message);
            }
        }

        if (context.Operation.Value.Method.ReturnParameter!.ParameterType == typeof(void))
            return ValidationResult.Failed(ValidationSubject.Operation, "Operation must have return type to be testable");

        Type? returnParameterType;
        var actualOperationType = nonTypedOperationValidatedObject.ActualOperationType;

        if (context.OutputUnderlyingType != null)
        {
            if (actualOperationType == null)
                return ValidationResult.Ok(ValidationSubject.Operation);
            returnParameterType = context.OutputUnderlyingType;
        }
        else
        {
            returnParameterType = context.Operation.Value.Method.ReturnParameter.ParameterType;
        }

        if (returnParameterType != actualOperationType)
            return ValidationResult.Failed(ValidationSubject.Operation, "Operation return type is not the same as used generic type.");
        
        return ValidationResult.Ok(ValidationSubject.Operation);
    }
    
    // TODO Possibly, there should be IEnricher interface to do such an actions before validations in Run(). But since this is the only such an activity, avoid additional complexity.
    private static Delegate GetDelegateFromAttributedMethod(IEntryAssemblyProvider entryAssemblyProvider, IActivator activator)
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
        var methodClassCtor = methodClassType!.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        var assemblyMethodClassCtorOfTestSuite = methodClassCtor
            .FirstOrDefault(x => x.GetParameters().Length == 0);
        if (assemblyMethodClassCtorOfTestSuite == null)
            throw new InvalidOperationException($"{nameof(TestSuiteDelegateAttribute)} has been defined for non-static method where declaring type do not have empty constructors, please add empty constructor or consider using static method.");

        var target = activator.CreateInstance(methodClassType);
        
        return assemblyMethodOfTestSuite.CreateDelegate(delegateType, target);
    }
}

public sealed class OperationValidatedObject(Type? actualOperationType) : IValidatedObject
{
    public Type? ActualOperationType { get; } = actualOperationType;
}