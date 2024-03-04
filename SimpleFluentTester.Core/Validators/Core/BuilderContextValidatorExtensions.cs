using System;
using SimpleFluentTester.Entities;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Validators.Core;

public static class BuilderContextValidatorExtensions
{
    public static TestSuiteBuilderContext<TOutput> RegisterValidator<TOutput>(
        this TestSuiteBuilderContext<TOutput> context,
        Type validatorType, 
        IValidatedObject validatedObject)
    {
        var validatorInvoker = CreateValidationInvoker(validatorType, context, validatedObject);
        context.Validators.Add(validatorInvoker);
        return context;
    }
    
    public static TestCase<TOutput> RegisterValidator<TOutput>(
        this TestCase<TOutput> testCase,
        TestSuiteBuilderContext<TOutput> context,
        Type validatorType, 
        IValidatedObject validatedObject)
    {
        var validatorInvoker = CreateValidationInvoker(validatorType, context, validatedObject);
        testCase.Validators.Add(validatorInvoker);
        return testCase;
    }

    private static ValidationInvoker<TOutput> CreateValidationInvoker<TOutput>(Type validatorType,
        TestSuiteBuilderContext<TOutput> context,
        IValidatedObject validatedObject)
    {
        var validator = (IValidator)Activator.CreateInstance(validatorType);
        return new ValidationInvoker<TOutput>(validator, context, validatedObject);
    }
}