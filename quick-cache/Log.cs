using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("quick-cache-test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

/// <summary>
/// Represents a log that stores values associated with unique log positions.
/// </summary>
internal class Log : ILog
{
    // A thread-safe dictionary to store values against log positions.
    private ConcurrentDictionary<ulong, ValueType> cache = new ConcurrentDictionary<ulong, System.ValueType>();

    /// <summary>
    /// Adds a value to the log at the specified log position.
    /// </summary>
    /// <param name="logPosition">The log position to add the value at.</param>
    /// <param name="value">The value to be added.</param>
    /// <typeparam name="T">The type of the value, constrained to value types.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if the log position already exists or the value cannot be added.</exception>
    public void AddValue<T>(ulong logPosition, T value) where T : struct
    {
        if (cache.ContainsKey(logPosition))
            throw new InvalidOperationException("Log position already exists");

        var res = cache.TryAdd(logPosition, value);
        if (!res)
            throw new InvalidOperationException("Value was not added");
    }

    /// <summary>
    /// Retrieves a value from the log based on the log position.
    /// </summary>
    /// <param name="logPosition">The log position to retrieve the value from.</param>
    /// <returns>The value at the specified log position or null if not found.</returns>
    /// <typeparam name="T">The type of the value, constrained to value types.</typeparam>
    public Nullable<T> GetValue<T>(ulong logPosition) where T : struct
    {
        var res = cache.TryGetValue(logPosition, out ValueType? value);

        return res ? (T)value! : null;
    }

    /// <summary>
    /// Removes a value from the log at the specified log position.
    /// </summary>
    /// <param name="logPosition">The log position of the value to be removed.</param>
    public void RemoveValue(ulong logPosition)
    {
        cache.TryRemove(logPosition, out ValueType? value);
    }

    /// <summary>
    /// Retrieves all log positions currently stored in the log.
    /// </summary>
    /// <returns>An array of log positions.</returns>
    public ulong[] GetAllPositions()
    {
        return cache.Keys.ToArray();
    }

    /// <summary>
    /// Clears all values and positions from the log.
    /// </summary>
    public void Clear()
    {
        cache.Clear();
    }
}
