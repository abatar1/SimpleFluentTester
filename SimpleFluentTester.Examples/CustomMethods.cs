namespace SimpleFluentTester.Examples;

internal static class CustomMethods
{
    [TestSuiteDelegate]
    internal static int Adder(int number1, int number2)
    {
        return number1 + number2;
    }
}