using System.Reflection;

namespace SimpleFluentTester.TestRun;

internal sealed class EntryAssemblyProvider : IEntryAssemblyProvider
{
    public Assembly? Get() => Assembly.GetEntryAssembly();
}