using System.Reflection;

namespace SimpleFluentTester.Helpers
{
    internal sealed class EntryAssemblyProvider : IEntryAssemblyProvider
    {
        public Assembly? Get() => Assembly.GetEntryAssembly();
    }
}