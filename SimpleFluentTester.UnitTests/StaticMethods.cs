using System.Reflection;

namespace SimpleFluentTester.UnitTests;

internal static class StaticMethods
{
    internal static int Adder(int number1, int number2)
    {
        return number1 + number2;
    }

    public static MethodInfo AdderMethodInfo => typeof(StaticMethods).GetMethod(nameof(Adder), BindingFlags.Static | BindingFlags.NonPublic) 
                                                ?? throw new Exception("Couldn't resolve MethodInfo for Added() method, possibly visibility of method has been changed so check binding flags");
    
    internal static void Empty(int number1, int number2)
    {
    }
    
    internal static int AdderThrowsCustomException(int number1, int number2)
    {
        throw new CustomException();
    }
}

internal class CustomException : Exception;