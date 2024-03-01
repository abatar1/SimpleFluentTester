using System;
using SimpleFluentTester.Entities;
using SimpleFluentTester.TestRun;

namespace SimpleFluentTester.Validators.Core;

public static class BuilderContextValidatorExtensions
{
    public static TestRunBuilderContext<TOutput> RegisterValidator<TOutput>(
        this TestRunBuilderContext<TOutput> context,
        Type validatorType, 
        IValidatedObject validatedObject)
    {
        var validatorInvoker = CreateValidationInvoker(validatorType, context, validatedObject);
        context.Validators.Add(validatorInvoker);
        return context;
    }
    
    public static TestCase<TOutput> RegisterValidator<TOutput>(
        this TestCase<TOutput> testCase,
        TestRunBuilderContext<TOutput> context,
        Type validatorType, 
        IValidatedObject validatedObject)
    {
        var validatorInvoker = CreateValidationInvoker(validatorType, context, validatedObject);
        testCase.Validators.Add(validatorInvoker);
        return testCase;
    }

    private static ValidationInvoker<TOutput> CreateValidationInvoker<TOutput>(Type validatorType,
        TestRunBuilderContext<TOutput> context,
        IValidatedObject validatedObject)
    {
        var validator = (IValidator)Activator.CreateInstance(validatorType);
        return new ValidationInvoker<TOutput>(validator, context, validatedObject);
    }
}