using Microsoft.Extensions.Configuration;
using System.Reactive.Linq;

/// <summary>
/// Represents an object cache that stores values associated with unique keys. in a thread-safe manner.
/// The cache can be observed for events related to cache entry removal.
/// </summary>
public class QuickCache : ICache
{
    // Holds the singleton instance of QuickCache.
    private static QuickCache? _instance = null;

    // Provides thread-safe access to the singleton instance.
    private static readonly object _lock = new object();

    // Dependencies for managing cache entries.
    private readonly ILog _log;
    private readonly ILogPosition _logPosition;
    private readonly ILogHash _logHash;
    private readonly IEventQueue _internalEventQueue;
    private readonly IEventQueue _externalEventQueue;
    private readonly IConfiguration _configuration;
    private readonly ILogManager _logManager;

    /// <summary>
    /// Subscribes to and filters events related to cache entry removal.
    /// </summary>
    public IObservable<Event> Observable => this._externalEventQueue.Observe();

    /// <summary>
    /// Initializes a new instance of the QuickCache class with default dependencies.
    /// </summary>
    internal QuickCache()
    {
        this._log = new Log();
        this._logPosition = new LogPosition();
        this._logHash = new LogHash(new EventQueue());
        this._internalEventQueue = new EventQueue();
        this._externalEventQueue = new EventQueue();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();
    }

    /// <summary>
    /// Initializes a new instance of the QuickCache class with specified dependencies.
    /// </summary>
    internal QuickCache(ILog log, ILogPosition logPosition, ILogHash logHash, IEventQueue internalEventQueue, IEventQueue externalEventQueue, ILogManager logManager, IConfiguration configuration)
    {
        this._log = log;
        this._logPosition = logPosition;
        this._logHash = logHash;
        this._internalEventQueue = internalEventQueue;
        this._externalEventQueue = externalEventQueue;
        this._configuration = configuration;
        this._logManager = logManager;

        var logThresholdStr = configuration.GetSection("quick-cache")["log-threshold"];
        var exists = int.TryParse(logThresholdStr, out int logThreshold);

        if (exists)
        {
            this._logManager.LogThreshHold = logThreshold;
        }

    }

    /// <summary>
    /// Retrieves the singleton instance of QuickCache with default dependencies.
    /// </summary>
    public static QuickCache GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new QuickCache();
                }
            }
        }
        return _instance;
    }

    /// <summary>
    /// Adds a value to the cache with the specified key.
    /// </summary>
    public bool Set<T>(string key, T value, bool force = false) where T : struct
    {
        var newLogPosition = this._logPosition.GetNewPosition();
        if (newLogPosition == null) return false;

        this._log.AddValue(newLogPosition!.Value, value);
        this._logHash.Set(key, newLogPosition!.Value);

        if (!force) return true;

        var currentPosition = this._logHash.GetLogPosition(key);

        return currentPosition != null && currentPosition.Value == newLogPosition.Value;
    }

    /// <summary>
    /// Retrieves a value from the cache based on the specified key.
    /// </summary>
    public Nullable<T> Get<T>(string key) where T : struct
    {
        var logPosition = this._logHash.GetLogPosition(key);
        if (logPosition == null) return null;

        return this._log.GetValue<T>(logPosition.Value);

    }

    /// <summary>
    /// Deletes the value associated with the specified key from the cache.
    /// </summary>
    public bool Delete(string key)
    {
        this._logHash.RemoveKey(key, true);

        return true;
    }
}
