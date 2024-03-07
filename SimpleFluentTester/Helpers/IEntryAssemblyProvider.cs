using System.Reflection;

namespace SimpleFluentTester.Helpers;

public interface IEntryAssemblyProvider
{
    Assembly? Get();
}