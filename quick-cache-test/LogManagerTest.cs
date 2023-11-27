[TestClass]
public class LogManagerTest
{
    [TestMethod]
    public void LogManager_ShouldHandleNewItemEvent()
    {
        // Setup mocks for EventQueue, Log, and LogHash
        var threshold = 3;
        var eventQueue = new EventQueue();
        var log = new Log();
        var logHash = new LogHash(eventQueue);
        var logManager = new LogManager(eventQueue, log, logHash);
        logManager.LogThreshHold = threshold;
        // Trigger a NewItem event

        log.AddValue(1, 1234);
        logHash.Set("someKey1", 1);

        log.AddValue(2, 1234);
        logHash.Set("someKey2", 1);

        log.AddValue(3, 1234);
        logHash.Set("someKey3", 1);

        log.AddValue(4, 1234);
        logHash.Set("someKey4", 1);

        // Assert that LogManager reacts correctly, e.g., by checking if items are added to Log/LogHash

        eventQueue.Observe().Subscribe(e => { },
            () =>
            {
                //Assert.AreEqual(threshold, log.GetAllPositions().Count());
            });

        eventQueue.Dispose();

    }
}