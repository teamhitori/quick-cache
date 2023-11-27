
/// <summary>
/// Represents an object cache that stores values associated with unique keys. in a thread-safe manner.
/// The cache can be observed for events related to cache entry removal.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Deletes the value associated with the specified key from the cache.
    /// </summary>
    bool Delete(string key);

    /// <summary>
    /// Retrieves a value from the cache based on the specified key.
    /// </summary>
    T? Get<T>(string key) where T : struct;

    /// <summary>
    /// Subscribes to and filters events related to cache entry removal.
    /// </summary>
    IObservable<Event> Observable { get; }

    /// <summary>
    /// Adds a value to the cache with the specified key.
    /// </summary>
    bool Set<T>(string key, T value, bool force = false) where T : struct;
}