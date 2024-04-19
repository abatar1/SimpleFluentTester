namespace SimpleFluentTester.TestSuite.ComparedObject;

public interface IComparedObjectFactory
{
    IComparedObject Wrap(object? obj);
    
    IComparedObject[] WrapMany(object?[] objects);
}