namespace SimpleFluentTester.UnitTests;

[CollectionDefinition("NonParallelCollection", DisableParallelization = true)]
public class NonParallelCollection : ICollectionFixture<object>;