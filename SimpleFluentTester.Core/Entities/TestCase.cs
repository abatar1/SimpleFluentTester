﻿using System.Collections.Generic;
using SimpleFluentTester.Validators.Core;

namespace SimpleFluentTester.Entities;

public sealed class TestCase<TOutput>(object?[] inputs, 
    TOutput? expected, 
    LazyAssert<TOutput> assert, 
    int number)
{
    public LazyAssert<TOutput> Assert { get; } = assert;

    public ISet<ValidationInvoker<TOutput>> Validators { get; } = new HashSet<ValidationInvoker<TOutput>>();
    
    public object?[] Inputs { get; } = inputs;
    
    public TOutput? Expected { get; } = expected;
    
    public int Number { get; } = number;
}