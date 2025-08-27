using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleFluentTester.Helpers;

internal static class CollectionHelper
{
    public static bool IsCollection(Type? type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }
    
    public static Type? GetCollectionElementType(Type? collectionType)
    {
        if (collectionType == null)
            return null;
        
        if (collectionType.IsArray)
            return collectionType.GetElementType();

        if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return collectionType.GetGenericArguments()[0];

        var enumerableInterface = collectionType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        return enumerableInterface?.GetGenericArguments()[0];
    }
}