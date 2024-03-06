using System.Collections.Generic;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.TestCase;

public sealed class TestCase<TOutput>(object?[] inputs, TOutput? expected, int number)
{
    public ISet<IValidationInvoker> Validators { get; } = new HashSet<IValidationInvoker>();
    
    public object?[] Inputs { get; } = inputs;
    
    public TOutput? Expected { get; } = expected;
    
    public int Number { get; } = number;
}