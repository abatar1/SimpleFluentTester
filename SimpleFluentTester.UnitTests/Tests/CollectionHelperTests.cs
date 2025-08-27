using SimpleFluentTester.Helpers;

namespace SimpleFluentTester.UnitTests.Tests;

public sealed class CollectionHelperTests
{
    [Fact]
    public void CollectionHelper_IsCollection_ShouldBeValid()
    {
        // Assign
        var type1 = typeof(IEnumerable<>);
        var type2 = typeof(string);
        var type3 = typeof(int);
        
        // Act
        var isCollection1 = CollectionHelper.IsCollection(type1);
        var isCollection2 = CollectionHelper.IsCollection(type2);
        var isCollection3 = CollectionHelper.IsCollection(type3);

        // Assert
        Assert.True(isCollection1);
        Assert.False(isCollection2);
        Assert.False(isCollection3);
    }
    
    [Fact]
    public void CollectionHelper_GetCollectionElementType_ShouldBeValid()
    {
        // Assign
        var type2 = typeof(int[]);
        var type3 = typeof(IEnumerable<int>);
        var type4 = typeof(IList<int>);
        
        // Act
        var collectionType1 = CollectionHelper.GetCollectionElementType(null);
        var collectionType2 = CollectionHelper.GetCollectionElementType(type2);
        var collectionType3 = CollectionHelper.GetCollectionElementType(type3);
        var collectionType4 = CollectionHelper.GetCollectionElementType(type4);

        // Assert
        Assert.Null(collectionType1);
        Assert.Equal(typeof(int), collectionType2);
        Assert.Equal(typeof(int), collectionType3);
        Assert.Equal(typeof(int), collectionType4);
    }
}