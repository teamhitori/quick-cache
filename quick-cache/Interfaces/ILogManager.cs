/// <summary>
/// Manages log entries based on events received through an event queue.
/// </summary>
internal interface ILogManager
{
    /// <summary>
    /// The threshold number of log entries to maintain.
    /// </summary>
    int LogThreshHold { get; set; }
}