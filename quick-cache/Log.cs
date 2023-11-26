using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TeamHitori.QuickCache;

[assembly: InternalsVisibleTo("quick-cache-test")]
internal class Log
{
    ConcurrentDictionary<ulong, ValueType> cache = new ConcurrentDictionary<ulong, System.ValueType>();

    public void AddValue<T>(ulong logPosition, T value) where T: struct 
    {
        if (cache.ContainsKey(logPosition))
            throw new InvalidOperationException("Log position already exists");

        var res = cache.TryAdd(logPosition, value);
        if (!res)
            throw new InvalidOperationException("Value was not added");
    }

    public Nullable<T> GetValue<T>(ulong logPosition) where T : struct
    {
        var res = cache.TryGetValue(logPosition, out ValueType? value);

        return res ? (T)value! : null;
    }

    public void RemoveValue(ulong logPosition)
    {
        cache.TryRemove(logPosition, out ValueType? value);
    }
    public ulong[] GetAllPositions()
    {
        return cache.Keys.ToArray();
    }

    public void Clear()
    {
        cache.Clear();
    }

}
