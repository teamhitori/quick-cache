internal interface ILogPosition
{
    /// <summary>
    /// Generates a new, unique log position.
    /// </summary>
    /// <returns>The next log position, or null if the incremented value wraps around to zero.</returns>
    ulong? GetNewPosition();

    /// <summary>
    /// Resets the log position to its initial state.
    /// </summary>
    void Reset();
}