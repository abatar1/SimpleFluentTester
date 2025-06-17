using System;
using System.Collections;
using System.Linq;

namespace SimpleFluentTester.TestSuite.ComparedObject;

public sealed class ValueObject(object value, Type type) : IComparedObject
{
    public ComparedObjectVariety Variety => ComparedObjectVariety.Value;

    public Type Type { get; } = type;

    public object Value => value;
    
    public override string ToString()
    {
        if (!ValueObjectHelper.IsArray(Type)) 
            return Value.ToString();
        
        var objArray = (IEnumerable)Value;
        return $"[{string.Join(", ", objArray.Cast<object>().Select(x => x.ToString()))}]";
    }
}