/// <summary>
/// Manages log entries based on events received through an event queue.
/// </summary>
internal class LogManager : ILogManager
{
    private IEventQueue _internalEventQueue;
    private readonly IEventQueue _externalEventQueue;
    private readonly ILog _log;
    private readonly ILogHash _logHash;
    public int LogThreshHold { set; get; }

    /// <summary>
    /// Initializes a new instance of LogManager.
    /// </summary>
    /// <param name="internalEventQueue">The internal event queue that LogManager subscribes to.</param>
    /// <param name="externalEventQueue">The external event queue that LogManager publishes to.</param>
    /// <param name="log">The log to manage.</param>
    /// <param name="logHash">The log hash that maps keys to log positions.</param>
    /// <param name="logThreshHold">The threshold number of log entries to maintain.</param>
    public LogManager(IEventQueue internalEventQueue, IEventQueue externalEventQueue, ILog log, ILogHash logHash)
    {
        this._internalEventQueue = internalEventQueue;
        this._externalEventQueue = externalEventQueue;
        this._log = log;
        this._logHash = logHash;
        this.LogThreshHold = 10000;

        // Subscribing to the event queue to manage logs based on received events.
        this._internalEventQueue.Observe()
            .Subscribe(
                onNext: e => ManageLogs(e),
                onError: ex => Console.WriteLine($"Error: {ex.Message}"),
                onCompleted: () => Console.WriteLine("Completed"));
    }

    /// <summary>
    /// Handles incoming log events and manages the log accordingly.
    /// </summary>
    /// <param name="logEvent">The event to handle.</param>
    private void ManageLogs(Event logEvent)
    {
        if(logEvent.EventType == EventType.Remove)
        {
            _logHash.RemoveKey(logEvent.Key, false);
            this._externalEventQueue.RaiseEvent(EventType.Remove, logEvent.Key);
        }

        var activePositions = this._logHash.GetAllReferences().OrderBy(p => p.Value);
        var logPositions = this._log.GetAllPositions().OrderBy(p => p);

        // Find orphan log positions.
        var orphans = FindOrphans(logPositions, activePositions.Select(p => p.Value));

        foreach (var orphan in orphans)
        {
            this._log.RemoveValue(orphan);
        }

        //
        if (logEvent.EventType == EventType.Add)
        {
            var exceeds = activePositions.Count() - this.LogThreshHold;
            // Remove the oldest log entries if the threshold is exceeded.
            if (exceeds > 0)
            {
                var toRemove = activePositions.OrderBy(p => p.Value).Take(exceeds);

                foreach (var item in toRemove)
                {
                    this._log.RemoveValue(item.Value);
                    this._logHash.RemoveKey(item.Key, false);
                    this._externalEventQueue.RaiseEvent(EventType.Remove, item.Key);
                }
            }
        }
    }


    /// <summary>
    /// Compares log positions to LogHash and identifies the 'orphan' log positions.
    /// The method assumes both input enumerables are ordered. It iterates through both sets simultaneously 
    /// and finds elements that are not common to both, effectively finding the 'orphan' log positions.
    /// </summary>
    /// <param name="logPositions">An ordered enumerable of log positions.</param>
    /// <param name="activeLogPositions">An ordered enumerable of currently active log positions.</param>
    /// <returns>A list of integers representing the orphan log positions.</returns>
    List<ulong> FindOrphans(IEnumerable<ulong> logPostions, IEnumerable<ulong> activeLogPositions)
    {
        List<ulong> nonMatching = new List<ulong>();

        using (var enumLog = logPostions.GetEnumerator())
        using (var enumActive = activeLogPositions.GetEnumerator())
        {
            bool hasMoreLog = enumLog.MoveNext();
            bool hasMoreActive = enumActive.MoveNext();

            while (hasMoreLog && hasMoreActive)
            {
                if (enumLog.Current == enumActive.Current)
                {
                    hasMoreLog = enumLog.MoveNext();
                    hasMoreActive = enumActive.MoveNext();
                }
                else if (enumLog.Current < enumActive.Current)
                {
                    nonMatching.Add(enumLog.Current);
                    hasMoreLog = enumLog.MoveNext();
                }
                else
                {
                    nonMatching.Add(enumActive.Current);
                    hasMoreActive = enumActive.MoveNext();
                }
            }

            // Add remaining elements from enumerable1
            while (hasMoreLog)
            {
                nonMatching.Add(enumLog.Current);
                hasMoreLog = enumLog.MoveNext();
            }
        }

        return nonMatching;
    }
}

