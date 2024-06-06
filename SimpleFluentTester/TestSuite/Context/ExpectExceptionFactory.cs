using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestSuite.Context;

public static class ExpectExceptionFactory
{
    private static readonly IDictionary<Type, List<ParameterInfo[]>> ParameterInfoMap = new Dictionary<Type, List<ParameterInfo[]>>();
    
    /// <summary>
    /// Converts input exception as an expected object, validated that it could be constructed from the given type and message if provided.
    /// </summary>
    public static ValidatedObject Create(
        ITestSuiteContextContainer container,
        Type exceptionType,
        string? message = null)
    {
        if (!ParameterInfoMap.TryGetValue(exceptionType, out var publicConstructorParams))
        {
            publicConstructorParams = exceptionType
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.GetParameters())
                .ToList();
            ParameterInfoMap[exceptionType] = publicConstructorParams;
        }

        if (!string.IsNullOrWhiteSpace(message))
        {
            if (!HasStringCtor(publicConstructorParams))
                return ValidationFailed($"{exceptionType} do not have public .ctor() with string parameter");
            return TryCreateException(container, exceptionType, message);
        }
        
        return TryCreateException(container, exceptionType);
    }

    private static ValidatedObject ValidationFailed(string message)
    {
        var validationResults = new List<ValidationResult> { ValidationResult.NonValid(ValidationSubject.Expect, message) };
        return new ValidatedObject(new NullObject(), validationResults);
    }

    private static bool HasStringCtor(IEnumerable<ParameterInfo[]> publicConstructorParams)
    {
        return publicConstructorParams
            .Any(parameters =>
            {
                var parameter = parameters.SingleOrDefault();
                return parameter != null && parameter.ParameterType == typeof(string);
            });
    }

    private static ValidatedObject TryCreateException(
        ITestSuiteContextContainer container,
        Type exceptionType,
        string? message = null)
    {
        object exception;

        if (string.IsNullOrWhiteSpace(message))
            exception = container.Context.Activator.CreateInstance(exceptionType);
        else
            exception = container.Context.Activator.CreateInstance(exceptionType, message);
        
        var wrappedException = ComparedObjectFactory.Wrap(exception);
        
        return new ValidatedObject(wrappedException);
    }
}