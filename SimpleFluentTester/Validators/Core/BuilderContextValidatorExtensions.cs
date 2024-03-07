using System;
using System.Collections.Generic;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite.Context;

namespace SimpleFluentTester.Validators.Core;

internal static class BuilderContextValidatorExtensions
{
    public static TestCase<TOutput> RegisterValidator<TOutput>(
        this TestCase<TOutput> testCase,
        Type validatorType,
        IValidatedObject validatedObject)
    {
        var validatorInvoker = CreateValidationInvoker(validatorType, validatedObject);
        testCase.Validators.Add(validatorInvoker);
        return testCase;
    }

    public static ITestSuiteBuilderContext<TOutput> InvokeValidation<TOutput>(
        this ITestSuiteBuilderContext<TOutput> context,
        Type validatorType,
        IValidatedObject validatedObject)
    {
        var validationInvoker = CreateValidationInvoker(validatorType, validatedObject);
        var validationResult = validationInvoker.Invoke(context);
        context.AddValidation(validationResult);
        return context;
    }

    public static ITestSuiteBuilderContext<TOutput> AddValidation<TOutput>(
        this ITestSuiteBuilderContext<TOutput> context,
        ValidationResult validationResult)
    {
        if (context.Validations.ContainsKey(validationResult.Subject))
            context.Validations[validationResult.Subject].Add(validationResult);
        else
            context.Validations[validationResult.Subject] = new List<ValidationResult> { validationResult };
        return context;
    }

    private static IValidationInvoker CreateValidationInvoker(
        Type validatorType,
        IValidatedObject validatedObject)
    {
        var validator = (IValidator)Activator.CreateInstance(validatorType);
        return new ValidationInvoker(validator, validatedObject);
    }
}