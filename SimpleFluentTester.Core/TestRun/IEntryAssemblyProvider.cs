using System.Reflection;

namespace SimpleFluentTester.TestRun;

public interface IEntryAssemblyProvider
{
    Assembly? Get();
}