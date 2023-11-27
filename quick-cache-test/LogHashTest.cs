using Moq;

[TestClass]
public class LogHashTest
{
    [TestMethod]
    public void GetLogPosition_ShouldRetrieveCorrectPositionForExistingKey()
    {
        // Arrange
        var mockEventQueue = new Mock<IEventQueue>();
        var logHash = new LogHash(mockEventQueue.Object);
        string key = "testKey";
        ulong expectedPosition = 123; // Example position
        logHash.Set(key, expectedPosition);

        // Act
        var actualPosition = logHash.GetLogPosition(key);

        // Assert
        Assert.AreEqual(expectedPosition, actualPosition);
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Touch, "testKey"), Times.AtLeastOnce());
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Add, "testKey"), Times.AtLeastOnce());
    }

    [TestMethod]
    public void GetLogPosition_ShouldReturnNullForNonExistentKey()
    {
        // Arrange
        var mockEventQueue = new Mock<IEventQueue>();
        var logHash = new LogHash(mockEventQueue.Object);
        string nonExistentKey = "nonExistentKey";

        // Act
        var position = logHash.GetLogPosition(nonExistentKey);

        // Assert
        Assert.IsNull(position);
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Touch, "testKey"), Times.Never());
    }

    [TestMethod]
    public void GetLogPosition_ShouldReturnNullAfterKeyIsRemoved()
    {
        // Arrange
        var mockEventQueue = new Mock<IEventQueue>();
        var logHash = new LogHash(mockEventQueue.Object);
        string key = "testKey";
        logHash.Set(key, 123);
        logHash.RemoveKey(key);

        // Act
        var position = logHash.GetLogPosition(key);

        // Assert
        Assert.IsNull(position);
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Remove, "testKey"), Times.Once());
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Add, "testKey"), Times.AtLeastOnce());
    }

    [TestMethod]
    public async Task GetLogPosition_ShouldBeThreadSafeForConcurrentAccess()
    {
        // Arrange
        var mockEventQueue = new Mock<IEventQueue>();
        var logHash = new LogHash(mockEventQueue.Object);
        string key = "testKey";
        ulong expectedPosition = 123;
        logHash.Set(key, expectedPosition);
        var tasks = new List<Task<ulong?>>();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            tasks.Add(Task.Run(() => logHash.GetLogPosition(key)));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        foreach (var task in tasks)
        {
            Assert.AreEqual(expectedPosition, task.Result);
        }
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Touch, "testKey"), Times.AtLeastOnce());
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Add, "testKey"), Times.AtLeastOnce());
    }

    [TestMethod]
    public void GetLogPosition_ShouldDistinguishSimilarKeys()
    {
        // Arrange
        var mockEventQueue = new Mock<IEventQueue>();
        var logHash = new LogHash(mockEventQueue.Object);
        string key1 = "key";
        string key2 = "key2";
        ulong position1 = 123;
        ulong position2 = 456;
        logHash.Set(key1, position1);
        logHash.Set(key2, position2);

        // Act
        var retrievedPosition1 = logHash.GetLogPosition(key1);
        var retrievedPosition2 = logHash.GetLogPosition(key2);

        // Assert
        Assert.AreEqual(position1, retrievedPosition1);
        Assert.AreEqual(position2, retrievedPosition2);
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Touch, "key"), Times.AtLeastOnce());
        mockEventQueue.Verify(queue => queue.RaiseEvent(EventType.Add, "key"), Times.AtLeastOnce());
    }



}
