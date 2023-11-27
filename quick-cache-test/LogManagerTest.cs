[TestClass]
public class LogManagerTest
{
    [TestMethod]
    public void LogManager_ShouldRespectThresholdLimit()
    {
        // Arrange
        var threshold = 2;
        var internalEventQueue = new EventQueue();
        var externalEventQueue = new EventQueue();
        var log = new Log();
        var logHash = new LogHash(internalEventQueue);
        var logManager = new LogManager(internalEventQueue, externalEventQueue, log, logHash);
        logManager.LogThreshHold = threshold;
        

        // Act
        log.AddValue(1, 1234);
        logHash.Set("someKey1", 1);

        log.AddValue(2, 1234);
        logHash.Set("someKey2", 2);

        log.AddValue(3, 1234);
        logHash.Set("someKey3", 3);

        // Assert that LogManager reacts correctly, e.g., by checking if items are added to Log/LogHash
        externalEventQueue.Observe().Subscribe(
            e =>
            {
                Assert.AreEqual(EventType.Remove, e.EventType);
                Assert.AreEqual(2, logHash.GetAllReferences().Count());
                Assert.AreEqual(2, log.GetAllPositions().Count());
            },
            () => {});
    }

    [TestMethod]
    public void LogManager_ShouldRespectThresholdAndTidyLogsCorrectly()
    {
        // Arrange
        var threshold = 2;
        var internalEventQueue = new EventQueue();
        var externalEventQueue = new EventQueue();
        var log = new Log();
        var logHash = new LogHash(internalEventQueue);
        var logManager = new LogManager(internalEventQueue, externalEventQueue, log, logHash);
        logManager.LogThreshHold = threshold;

        // Act
        log.AddValue(1, 1234);
        logHash.Set("someKey1", 1);

        log.AddValue(2, 5678);
        logHash.Set("someKey1", 2);

        log.AddValue(3, 1234);
        logHash.Set("someKey2", 3);

        log.AddValue(4, 1234);
        logHash.Set("someKey3", 4);

        // Assert that LogManager reacts correctly by removing the oldest item
        // and tidying up the log and logHash
        externalEventQueue.Observe().Subscribe(
            e =>
            {
                Assert.AreEqual(EventType.Remove, e.EventType);
                Assert.AreEqual(2, logHash.GetAllReferences().Count());
                Assert.AreEqual(2, log.GetAllPositions().Count());

                var keyPos1 = logHash.GetLogPosition("someKey1");
                var keyPos2 = logHash.GetLogPosition("someKey2");
                var keyPos3 = logHash.GetLogPosition("someKey3");

                Assert.AreEqual(null, keyPos1);
                Assert.AreEqual((ulong)3, keyPos2!.Value);
                Assert.AreEqual((ulong)4, keyPos3!.Value);
            },
            () => { });
    }
}