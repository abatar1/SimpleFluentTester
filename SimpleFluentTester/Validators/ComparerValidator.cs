using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFluentTester.TestSuite.ComparedObject;
using SimpleFluentTester.TestSuite.Context;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Validators;

internal sealed class ComparerValidator : BaseValidator<EmptyValidatedObject>
{
    public override ISet<Type> AllowedTypes => new HashSet<Type>([ValidatedTypes.Context]);
    public override ValidationSubject Subject => ValidationSubject.Comparer;

    public override ValidationResult Validate(
        IValidated validated, 
        IValidatedObject validatedObject)
    {
        var context = CastValidated<ITestSuiteContext>(validated);

        var testCaseExpectedObjects = context.TestCases
            .Where(x => x.Expected.Type != null)
            .Select(x => x.Expected)
            .Where(x => x.Variety == ComparedObjectVariety.Value)
            .ToLookup(x => x.Type);

        if (testCaseExpectedObjects.Count == 0)
            return Ok();
        
        if (testCaseExpectedObjects.Count > 1)
            return NonValid("Expected types more than one in TestCase collection");
            
        var testCaseExpectedObject = (ValueObject) testCaseExpectedObjects.First().First();

        if (context.Comparer != null)
        {
            if (context.Comparer?.Method.ReturnParameter?.ParameterType != typeof(bool))
                return NonValid(
                    $"Return type of the custom comparer is not bool but {context.Comparer?.Method.ReturnParameter?.ParameterType}, something went wrong during initialization");

            var parameters = context.Comparer.Method.GetParameters();
            if (parameters.Length != 2)
                return NonValid(
                    $"Custom comparer has {parameters.Length} parameters, but should has 2, something went wrong during initialization");

            var paramType = parameters[0].ParameterType;

            if (paramType != parameters[1].ParameterType)
                return NonValid("Comparer has not the same input parameter type, something went wrong during initialization");
            
            if (testCaseExpectedObject != null && paramType != testCaseExpectedObject.Type)
                return NonValid($"Test case type was {testCaseExpectedObject.Type}, but comparer type is {paramType}");
        }
        else
        {
            var interfaceType = typeof(IEquatable<>).MakeGenericType(testCaseExpectedObject.Type);
            if (!interfaceType.IsAssignableFrom(testCaseExpectedObject.Type))
                return NonValid($"{testCaseExpectedObject.Type} type should be assignable from {typeof(IEquatable<>).Name} or comparer should be defined");
        }
           
        return Ok();
    }
}