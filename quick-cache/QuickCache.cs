using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TeamHitori.QuickCache;
using TeamHitori.QuickCache.Interfaces;


public class QuickCache : ICache
{
    private static readonly object _lock = new object();
    private readonly ILog _log;
    private readonly ILogPosition _logPosition;
    private readonly ILogHash _logHash;
    private static QuickCache _instance = null;
    public static QuickCache Instance { get; set; }

    internal QuickCache(ILog log, ILogPosition logPosition, ILogHash logHash)
    {
        this._log = log;
        this._logPosition = logPosition;
        this._logHash = logHash;
    }

    internal static QuickCache GetInstance(ILog log, ILogPosition logPosition, ILogHash logHash)
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new QuickCache(log, logPosition, logHash);
                }
            }
        }
        return _instance;
    }

    public static QuickCache GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new QuickCache(new Log(), new LogPosition(), new LogHash());
                }
            }
        }
        return _instance;
    }

    public bool Set<T>(string key, T value, bool force = false) where T : struct
    {
        var newLogPosition = this._logPosition.GetNewPosition();
        if (newLogPosition == null) return false;

        this._log.AddValue(newLogPosition!.Value, value);
        this._logHash.Set(key, newLogPosition!.Value);

        if (!force) return true;

        var currentPosition = this._logHash.GetLogPosition(key);

        return currentPosition.Value == newLogPosition.Value;
    }
    public Nullable<T> Get<T>(string key) where T : struct
    {
        var logPosition = this._logHash.GetLogPosition(key);
        if (logPosition == null) return null;

        return this._log.GetValue<T>(logPosition.Value);

    }
    public bool Delete(string key)
    {
        this._logHash.RemoveKey(key);

        return true;
    }
    public IObservable<Event> Observe()
    {
        return null;
    }
}