﻿using SimpleFluentTester.TestSuite;

namespace SimpleFluentTester.Examples;

internal static class CustomMethods
{
    [TestSuiteDelegate]
    internal static int Adder(int number1, int number2)
    {
        return number1 + number2;
    }
    
    internal static CustomValue CustomAdder(CustomValue number1, CustomValue number2)
    {
        return new CustomValue(number1.Value + number2.Value);
    }
}