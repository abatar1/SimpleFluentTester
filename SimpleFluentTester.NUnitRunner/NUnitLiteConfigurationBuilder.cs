namespace SimpleFluentTester.NUnitRunner;

internal sealed class NUnitLiteConfigurationBuilder(NUnitLiteConfiguration configuration)
{
    public string[] Build()
    {
        var stringBuilder = new List<string>();

        stringBuilder.Add("--noheader");

        if (!configuration.WriteResultToFile)
        {
            stringBuilder.Add("--noresult");
        }

        return stringBuilder.ToArray();
    }
}