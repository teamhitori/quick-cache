using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("quick-cache-test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] 
internal interface ILog
{
    void AddValue<T>(ulong logPosition, T value) where T : struct;
    void Clear();
    ulong[] GetAllPositions();
    T? GetValue<T>(ulong logPosition) where T : struct;
    void RemoveValue(ulong logPosition);
}