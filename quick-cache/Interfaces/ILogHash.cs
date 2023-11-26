namespace TeamHitori.QuickCache.Interfaces
{
    internal interface ILogHash
    {
        ulong[] GetAllReferences();
        ulong? GetLogPosition(string key);
        void RemoveKey(string key);
        void Set(string key, ulong logPosition);
    }
}