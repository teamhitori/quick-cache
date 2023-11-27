using Moq;

[TestClass]
public class QuickCacheTest
{
    [TestMethod]
    public void Set_ShouldAddValueCorrectly()
    {
        // Arrange
        var mockLogPosition = new Mock<ILogPosition>();
        var mockLog = new Mock<ILog>();
        var mockLogHash = new Mock<ILogHash>();
        var mockEventQueue = new Mock<IEventQueue>();
        var quickCache = new QuickCache(mockLog.Object, mockLogPosition.Object, mockLogHash.Object, mockEventQueue.Object);
        string key = "testKey";
        var value = 1234;

        // Act
        mockLogPosition.Setup(logPosition => logPosition.GetNewPosition()).Returns(1);
        quickCache.Set(key, value);

        // Assert
        mockLog.Verify(log => log.AddValue(1, value), Times.Once());
        mockLogHash.Verify(logHash => logHash.Set(key, 1), Times.Once());
    }

    [TestMethod]
    public void Set_ShouldUpdateValueForDuplicateKey()
    {
        // Arrange
        var mockLogPosition = new Mock<ILogPosition>();
        var mockLog = new Mock<ILog>();
        var mockLogHash = new Mock<ILogHash>();
        var mockEventQueue = new Mock<IEventQueue>();
        var quickCache = new QuickCache(mockLog.Object, mockLogPosition.Object, mockLogHash.Object, mockEventQueue.Object);
        string key = "testKey";
        var value1 = 1234;
        var value2 = 5678;

        ulong? position = 0;
        mockLogPosition.Setup(logPosition => logPosition.GetNewPosition()).Returns(() =>
        {
            return position++;
        });

        // Act
        quickCache.Set(key, value1);
        quickCache.Set(key, value2);

        // Assert
        mockLog.Verify(log => log.AddValue(It.IsAny<ulong>(), value2), Times.Once());
        Mock.Get(mockLogHash.Object).Verify(logHash => logHash.Set(key, 1), Times.Once());
    }


    [TestMethod]
    public void Set_ShouldGenerateNewLogPositionForNewKey()
    {
        // Arrange
        var mockLogPosition = new Mock<ILogPosition>();
        var mockLog = new Mock<ILog>();
        var mockLogHash = new Mock<ILogHash>();
        var mockEventQueue = new Mock<IEventQueue>();
        var quickCache = new QuickCache(mockLog.Object, mockLogPosition.Object, mockLogHash.Object, mockEventQueue.Object);
        string key = "newKey";
        var value = 1234;

        // Act
        quickCache.Set(key, value);

        // Assert
        mockLogPosition.Verify(logPos => logPos.GetNewPosition(), Times.Once());
    }

    [TestMethod]
    public void Set_ShouldCallLogHashSetWithCorrectParameters()
    {
        // Arrange
        var mockLogPosition = new Mock<ILogPosition>();
        var mockLog = new Mock<ILog>();
        var mockLogHash = new Mock<ILogHash>();
        var mockEventQueue = new Mock<IEventQueue>();
        var quickCache = new QuickCache(mockLog.Object, mockLogPosition.Object, mockLogHash.Object, mockEventQueue.Object);
        string key = "testKey";
        var value = 1234;
        ulong expectedPosition = 123;
        mockLogPosition.Setup(logPos => logPos.GetNewPosition()).Returns(expectedPosition);

        // Act
        quickCache.Set(key, value);

        // Assert
        mockLogHash.Verify(logHash => logHash.Set(key, expectedPosition), Times.Once());
    }

    [TestMethod]
    public async Task Set_ShouldBeThreadSafe()
    {
        // Arrange
        var mockLogPosition = new Mock<ILogPosition>();
        var mockLog = new Mock<ILog>();
        var mockLogHash = new Mock<ILogHash>();
        var mockEventQueue = new Mock<IEventQueue>();
        var quickCache = new QuickCache(mockLog.Object, mockLogPosition.Object, mockLogHash.Object, mockEventQueue.Object);

        mockLogPosition.Setup(logPos => logPos.GetNewPosition()).Returns(1);

        var tasks = new List<Task<int>>();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            tasks.Add(Task.Run(() => { quickCache.Set($"key{i}", 1234); return 1; }));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        Mock.Get(mockLogHash.Object).Verify(logHash => logHash.Set(It.IsAny<string>(), 1), Times.Exactly(1000));
    }



}