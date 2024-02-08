namespace SimpleFluentTester.UnitTests;

internal static class StaticMethods
{
    internal static int Adder(int number1, int number2)
    {
        return number1 + number2;
    }
    
    internal static void Empty(int number1, int number2)
    {
    }
    
    internal static int AdderThrowsCustomException(int number1, int number2)
    {
        throw new CustomException();
    }
}

internal class CustomException : Exception;