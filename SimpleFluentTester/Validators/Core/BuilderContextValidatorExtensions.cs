using System;
using SimpleFluentTester.TestCase;
using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Validators.Core;

internal static class BuilderContextValidatorExtensions
{
    public static TestCase<TOutput> RegisterValidator<TOutput>(
        this TestCase<TOutput> testCase,
        ITestSuiteBuilderContext<TOutput> context,
        Type validatorType, 
        IValidatedObject validatedObject)
    {
        var validatorInvoker = context.CreateValidationInvoker(validatorType, validatedObject);
        testCase.Validators.Add(validatorInvoker);
        return testCase;
    }

    public static ValidationResult InvokeValidation<TOutput>(
        this ITestSuiteBuilderContext<TOutput> context,
        Type validatorType,
        IValidatedObject validatedObject)
    {
        var validationInvoker = context.CreateValidationInvoker(validatorType, validatedObject);
        return validationInvoker.Invoke();
    }

    private static ValidationInvoker<TOutput> CreateValidationInvoker<TOutput>(
        this ITestSuiteBuilderContext<TOutput> context,
        Type validatorType,
        IValidatedObject validatedObject)
    {
        var validator = (IValidator)Activator.CreateInstance(validatorType);
        return new ValidationInvoker<TOutput>(validator, context, validatedObject);
    }
}