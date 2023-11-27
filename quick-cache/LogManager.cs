/// <summary>
/// Manages log entries based on events received through an event queue.
/// </summary>
internal class LogManager : ILogManager
{
    private IEventQueue _eventQueue;
    private readonly ILog _log;
    private readonly ILogHash _logHash;
    public int LogThreshHold { set; get; }

    /// <summary>
    /// Initializes a new instance of LogManager.
    /// </summary>
    /// <param name="eventQueue">The event queue to subscribe to.</param>
    /// <param name="log">The log to manage.</param>
    /// <param name="logHash">The log hash that maps keys to log positions.</param>
    /// <param name="logThreshHold">The threshold number of log entries to maintain.</param>
    public LogManager(IEventQueue eventQueue, ILog log, ILogHash logHash)
    {
        this._eventQueue = eventQueue;
        this._log = log;
        this._logHash = logHash;
        this.LogThreshHold = 10000;

        // Subscribing to the event queue to manage logs based on received events.
        this._eventQueue.Observe()
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
        switch (logEvent.EventType)
        {
            case EventType.Add:
                this.ManageAdd(logEvent);
                break;
            case EventType.Remove:
                break;
            case EventType.Clear:
                break;
            case EventType.Touch:
                break;
        }
    }

    /// <summary>
    /// Manages adding a log entry and ensures the total count doesn't exceed the threshold.
    /// </summary>
    /// <param name="logEvent">The Add event to handle.</param>
    private void ManageAdd(Event logEvent)
    {
        var positions = this._log.GetAllPositions();
        var exceeds = positions.Length - this.LogThreshHold;

        // Remove the oldest log entries if the threshold is exceeded.
        if (exceeds > 0)
        {
            var toRemove = positions.Order().Take(exceeds);

            foreach (var item in toRemove)
            {
                this._log.RemoveValue(item);
            }
        }
    }
}

