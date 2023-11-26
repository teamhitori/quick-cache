
public interface ICache
{
    bool Delete(string key);
    T? Get<T>(string key) where T : struct;
    IObservable<Event> Observe();
    bool Set<T>(string key, T value, bool force = false) where T : struct;
}