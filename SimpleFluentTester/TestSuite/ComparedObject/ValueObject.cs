using System;
using System.Collections;
using System.Linq;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public sealed class ValueObject(object value, Type type) : IComparedObject
{
    public ComparedObjectVariety Variety => ComparedObjectVariety.Value;

    public Type Type { get; } = type;

    public object Value => value;
    
    private readonly bool _isArray = typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    
    public override string ToString()
    {
        if (_isArray)
        {
            var objArray = (IEnumerable)Value;
            return $"[{string.Join(", ", objArray.Cast<object>().Select(x => x.ToString()))}]";
        }
        return Value.ToString();
    }
}