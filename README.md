# **Quick Cache** - The blazing-fast 🚀, thread-safe in-memory cache for .NET applications.

## Purpose
Quick Cache is designed to provide .NET developers with an efficient, easy-to-use in-memory caching solution. Perfect for applications handling large data sets or requiring rapid data retrieval, Quick Cache ensures thread safety and high performance.

## Features
- **Generic Data Storage**: Store any type of object.
- **Thread Safety**: Built for concurrent environments.
- **LRU Eviction Policy**: Automatic least recently used eviction.
- **Observable Events**: Subscribe to cache events for real-time updates.

## Getting Started
Add Quick Cache to your project using NuGet:

### Installation

```powershell
...
```

### Usage
#### Creating an Instance

```csharp
var cache = new QuickCache();
```

#### Adding Items to Cache
```csharp
cache.Set("myKey", new MyObject());

// short hand
cache["myKey"] = new Object();
```

#### Retrieving Items from Cache
```csharp
if (cache.Get("myKey", out MyObject value))
{
    // Use value
}

// short hand
var res = cache["myKey"];
if(res.HasValue)
{
  // use value
}
```

#### Deleting Items from Cache
```csharp
cache.Delete("myKey");
````

#### Observing Cache Events
```csharp
cache.Observe().Subscribe(event =>
{
    Console.WriteLine($"Event Type: {event.EventType}, Key: {event.Key}");
});
```
