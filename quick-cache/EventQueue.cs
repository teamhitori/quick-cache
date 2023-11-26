using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

internal class EventQueue : IDisposable, IEventQueue
{
    private ConcurrentBag<IObserver<Event>> _observers = new ConcurrentBag<IObserver<Event>>();

    private readonly IObservable<Event> _observable;

    public EventQueue()
    {
        this._observable = Observable.Create<Event>(
            observer =>
            {
                this._observers.Add(observer);

                return this; ;
            })
            .ObserveOn(ThreadPoolScheduler.Instance);
    }

    public void RaiseEvent(EventType eventType, string key)
    {
        Task.Run(() =>
        {
            foreach (var observer in _observers)
                observer.OnNext(new Event(eventType, key));
        });

    }

    public IObservable<Event> Observe()
    {
        return _observable;
    }


    public void Dispose()
    {

    }
}
