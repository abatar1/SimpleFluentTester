namespace SimpleFluentTester.UnitTests.Helpers;

internal static class TestHelpers
{
    public static void AssertWithMessage<TException>(Action action, string message)
        where TException : Exception
    {
        try
        {
            action.Invoke();
            Assert.Fail($"Exception {typeof(TException)} was not thrown.");
        }
        catch (TException e)
        {
            Assert.Equal(message, e.Message);
        }
    }
}