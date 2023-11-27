/// <summary>
/// Represents a log hash that maps keys to log positions and raises events based on actions.
/// </summary>
internal interface ILogHash
{
    /// <summary>
    /// Retrieves all log keys and positions currently stored in the hash.
    /// </summary>
    /// <returns>An array of all keys and log positions in the hash.</returns>
    KeyValuePair<string, ulong>[] GetAllReferences();

    /// <summary>
    /// Retrieves the log position associated with a given key and raises a 'Touch' event.
    /// </summary>
    /// <param name="key">The key to retrieve the log position for.</param>
    /// <returns>The log position if found; otherwise, null.</returns>
    ulong? GetLogPosition(string key);

    /// <summary>
    /// Removes a key and its associated log position from the hash and raises a 'Remove' event.
    /// </summary>
    /// <param name="key">The key to be removed from the hash.</param>
    void RemoveKey(string key, bool createEvent);

    /// <summary>
    /// Adds a key-log position mapping to the hash and raises an 'Add' event.
    /// </summary>
    /// <param name="key">The key to associate with the log position.</param>
    /// <param name="logPosition">The log position to map to the key.</param>
    void Set(string key, ulong logPosition);
}