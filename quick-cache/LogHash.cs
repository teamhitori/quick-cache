using System.Collections.Concurrent;

/// <summary>
/// Represents a log hash that maps keys to log positions and raises events based on actions.
/// </summary>
internal class LogHash : ILogHash
{
    // A thread-safe dictionary to map keys to log positions.
    private ConcurrentDictionary<string, ulong> _hash = new ConcurrentDictionary<string, ulong>();

    // The event queue to raise events when actions are performed on the log hash.
    private readonly IEventQueue _eventQueue;

    /// <summary>
    /// Initializes a new instance of the LogHash class with a specified event queue.
    /// </summary>
    /// <param name="eventQueue">The event queue to be used for raising events.</param>
    public LogHash(IEventQueue eventQueue)
    {
        this._eventQueue = eventQueue;
    }

    /// <summary>
    /// Retrieves the log position associated with a given key and raises a 'Touch' event.
    /// </summary>
    /// <param name="key">The key to retrieve the log position for.</param>
    /// <returns>The log position if found; otherwise, null.</returns>
    public ulong? GetLogPosition(string key)
    {
        var hasValue = this._hash.TryGetValue(key, out ulong value);
        if (hasValue)
        {
            this._eventQueue.RaiseEvent(EventType.Touch, key);
            return value;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Adds a key-log position mapping to the hash and raises an 'Add' event.
    /// </summary>
    /// <param name="key">The key to associate with the log position.</param>
    /// <param name="logPosition">The log position to map to the key.</param>
    public void Set(string key, ulong logPosition)
    {
        this._hash.TryAdd(key, logPosition);
        this._eventQueue.RaiseEvent(EventType.Add, key);
    }

    /// <summary>
    /// Removes a key and its associated log position from the hash and raises a 'Remove' event.
    /// </summary>
    /// <param name="key">The key to be removed from the hash.</param>
    public void RemoveKey(string key, bool createEvent)
    {
        this._hash.TryRemove(key, out ulong value);

        if(createEvent)
            this._eventQueue.RaiseEvent(EventType.Remove, key);
    }

    /// <summary>
    /// Retrieves all log keys and positions currently stored in the hash.
    /// </summary>
    /// <returns>An array of all log keys and positions in the hash.</returns>
    public KeyValuePair<string, ulong>[] GetAllReferences()
    {
        return this._hash.ToArray();
    }
}

