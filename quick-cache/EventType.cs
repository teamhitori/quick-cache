
/// <summary>
/// Defines the types of events that can occur within the system.
/// </summary>
public enum EventType
{
    /// <summary>
    /// Indicates an event where an item is added.
    /// </summary>
    Add,

    /// <summary>
    /// Indicates an event where an item is removed.
    /// </summary>
    Remove,

    /// <summary>
    /// Indicates an event where an item is accessed or updated.
    /// </summary>
    Touch,

    /// <summary>
    /// Indicates an event where all items are cleared.
    /// </summary>
    Clear
}