using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamHitori.QuickCache
{
    internal class LogHash
    {
        ConcurrentDictionary<string, ulong> _hash = new ConcurrentDictionary<string, ulong>();

        public ulong? GetLogPosition(string key)
        {
            var hasValue = this._hash.TryGetValue(key, out ulong value);
            return hasValue ? value : null;
        }
        public void Set(string key, ulong logPosition)
        {
            this._hash.TryAdd(key, logPosition);
        }

        public void RemoveKey(string key)
        {
            this._hash.TryRemove(key, out ulong value);
        }
        public ulong[] GetAllReferences()
        {
            return this._hash.Values.ToArray();
        }
    }
}
