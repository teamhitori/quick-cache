using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

/// <summary>
/// Represents an event queue that allows observers to subscribe to and receive event notifications.
/// Implements IDisposable to handle resource cleanup.
/// </summary>
internal class EventQueue : IDisposable, IEventQueue
{
    // Holds the list of observers subscribed to the event notifications.
    private ConcurrentBag<IObserver<Event>> _observers = new ConcurrentBag<IObserver<Event>>();

    // Represents the observable sequence of events.
    private readonly IObservable<Event> _observable;

    /// <summary>
    /// Constructs an instance of the EventQueue.
    /// </summary>
    public EventQueue()
    {
        // Initializes the observable and adds any observer to the observers collection.
        this._observable = Observable.Create<Event>(
            observer =>
            {
                this._observers.Add(observer);

                // Returning 'this' here signifies that the resource (EventQueue) is the one to be disposed.
                return this;
            })
            .ObserveOn(ThreadPoolScheduler.Instance); // Specifies the scheduler on which observers will receive notifications.
    }

    /// <summary>
    /// Raises an event to all subscribed observers.
    /// </summary>
    /// <param name="eventType">The type of the event being raised.</param>
    /// <param name="key">The key associated with the event.</param>
    public void RaiseEvent(EventType eventType, string key)
    {
        // Uses a background task to raise the event to avoid blocking the caller.
        Task.Run(() =>
        {
            foreach (var observer in _observers)
                observer.OnNext(new Event(eventType, key));
        });
    }

    /// <summary>
    /// Allows observers to subscribe to the event notifications.
    /// </summary>
    /// <returns>An observable sequence of events.</returns>
    public IObservable<Event> Observe()
    {
        return _observable;
    }

    /// <summary>
    /// Disposes of the EventQueue, signaling completion to all subscribed observers.
    /// </summary>
    public void Dispose()
    {
        // Signals to each observer that no further notifications will be sent.
        foreach (var item in _observers)
        {
            item.OnCompleted();
        }
    }
}