using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("quick-cache-test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
/// <summary>
/// Represents a log that stores values associated with unique log positions.
/// </summary>
internal interface ILog
{
    /// <summary>
    /// Adds a value to the log at the specified log position.
    /// </summary>
    /// <param name="logPosition">The log position to add the value at.</param>
    /// <param name="value">The value to be added.</param>
    /// <typeparam name="T">The type of the value, constrained to value types.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if the log position already exists or the value cannot be added.</exception>
    void AddValue<T>(ulong logPosition, T value) where T : struct;

    /// <summary>
    /// Clears all values and positions from the log.
    /// </summary>
    void Clear();

    /// <summary>
    /// Retrieves all log positions currently stored in the log.
    /// </summary>
    /// <returns>An array of log positions.</returns>
    ulong[] GetAllPositions();

    /// <summary>
    /// Retrieves a value from the log based on the log position.
    /// </summary>
    /// <param name="logPosition">The log position to retrieve the value from.</param>
    /// <returns>The value at the specified log position or null if not found.</returns>
    /// <typeparam name="T">The type of the value, constrained to value types.</typeparam>
    T? GetValue<T>(ulong logPosition) where T : struct;

    /// <summary>
    /// Removes a value from the log at the specified log position.
    /// </summary>
    /// <param name="logPosition">The log position of the value to be removed.</param>
    void RemoveValue(ulong logPosition);
}