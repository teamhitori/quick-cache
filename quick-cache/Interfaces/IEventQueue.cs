
internal interface IEventQueue
{
    /// <summary>
    /// Allows observers to subscribe to the event notifications.
    /// </summary>
    /// <returns>An observable sequence of events.</returns>
    IObservable<Event> Observe();

    /// <summary>
    /// Raises an event to all subscribed observers.
    /// </summary>
    /// <param name="eventType">The type of the event being raised.</param>
    /// <param name="key">The key associated with the event.</param>
    void RaiseEvent(EventType eventType, string key);
}