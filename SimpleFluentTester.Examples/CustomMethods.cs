using SimpleFluentTester.TestSuite;

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
    
    internal static string BrokenAdderMessage => "Broken adder";

    internal static int BrokenAdder(int number1, int number2)
    {
        throw new AdderException(BrokenAdderMessage);
    }
    
    internal static void VoidPositionalSeqAdder(int[] seq1, int[] seq2)
    {
        for (var i = 0; i < seq1.Length; i++)
        {
            seq1[i] += seq2[i];
        }
    }
    
    internal static int PositionalSeqAdder(int[] seq1, int[] seq2)
    {
        for (var i = 0; i < seq1.Length; i++)
        {
            seq1[i] += seq2[i];
        }
        return seq1[0] + seq2[0];
    }
}