using System.Reflection;

namespace SimpleFluentTester.TestRun;

public sealed class EntryAssemblyProvider : IEntryAssemblyProvider
{
    public Assembly? Get() => Assembly.GetEntryAssembly();
}