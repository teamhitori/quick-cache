using System.Threading;

/// <summary>
/// Manages the generation of unique log positions in a thread-safe manner.
/// </summary>
internal class LogPosition : ILogPosition
{
    // Holds the current position, used to generate unique log positions.
    private ulong _position;

    /// <summary>
    /// Initializes a new instance of the LogPosition class with an optional seed value.
    /// </summary>
    /// <param name="seed">The initial value of the log position (default is 0).</param>
    public LogPosition(ulong seed = 0)
    {
        _position = seed;
    }

    /// <summary>
    /// Generates a new, unique log position.
    /// </summary>
    /// <returns>The next log position, or null if the incremented value wraps around to zero.</returns>
    public ulong? GetNewPosition()
    {
        // Atomically increments _position to ensure thread safety.
        var value = Interlocked.Increment(ref _position);

        // If the value wraps around to 0, return null to indicate an overflow.
        return value == 0 ? null : value;
    }

    /// <summary>
    /// Resets the log position to its initial state.
    /// </summary>
    public void Reset()
    {
        // Atomically sets _position back to 0.
        Interlocked.Exchange(ref _position, 0);
    }
}
