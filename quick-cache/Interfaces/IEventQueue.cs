
internal interface IEventQueue
{
    IObservable<Event> Observe();
    void RaiseEvent(EventType eventType, string key);
}