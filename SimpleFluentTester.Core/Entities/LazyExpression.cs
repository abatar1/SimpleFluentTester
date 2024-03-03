using System;
using System.Linq.Expressions;

namespace SimpleFluentTester.Entities;

public sealed class LazyAssert<TOutput>
{
    private Assert<TOutput>? _assert;
    
    public LazyAssert(Expression<Func<Assert<TOutput>>> assertExpression)
    {
        AssertExpression = assertExpression;
    }

    public Assert<TOutput> Value
    {
        get
        {
            if (_assert != null)
                return _assert;

            _assert = AssertExpression.Compile().Invoke();
            IsValueCreated = true;
            return _assert;
        }
    } 
    
    public Expression<Func<Assert<TOutput>>> AssertExpression { get; }
    
    public bool IsValueCreated { get; private set; }
}