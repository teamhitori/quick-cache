
/// <summary>
/// Represents an event with a type and a key.
/// </summary>
/// <param name="EventType">The type of the event.</param>
/// <param name="Key">The key associated with the event, which can be used to identify or categorize the event.</param>
public record Event(EventType EventType, string Key);
