using Microsoft.Extensions.Configuration;
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
        var mockInternalEventQueue = new Mock<IEventQueue>();
        var mockEnternalEventQueue = new Mock<IEventQueue>();
        var mockLogManager = new Mock<ILogManager>();
        var inMemorySettings = new Dictionary<string, string?> { };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var quickCache = new QuickCache(
            mockLog.Object,
            mockLogPosition.Object,
            mockLogHash.Object,
            mockInternalEventQueue.Object,
            mockEnternalEventQueue.Object,
            mockLogManager.Object,
            configuration);
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
        var mockInternalEventQueue = new Mock<IEventQueue>();
        var mockEnternalEventQueue = new Mock<IEventQueue>();
        var mockLogManager = new Mock<ILogManager>();
        var inMemorySettings = new Dictionary<string, string?> { };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var quickCache = new QuickCache(
            mockLog.Object,
            mockLogPosition.Object,
            mockLogHash.Object,
            mockInternalEventQueue.Object,
            mockEnternalEventQueue.Object,
            mockLogManager.Object,
            configuration);
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
        var mockInternalEventQueue = new Mock<IEventQueue>();
        var mockEnternalEventQueue = new Mock<IEventQueue>();
        var mockLogManager = new Mock<ILogManager>();
        var inMemorySettings = new Dictionary<string, string?> { };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var quickCache = new QuickCache(
            mockLog.Object,
            mockLogPosition.Object,
            mockLogHash.Object,
            mockInternalEventQueue.Object,
            mockEnternalEventQueue.Object,
            mockLogManager.Object,
            configuration);
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
        var mockInternalEventQueue = new Mock<IEventQueue>();
        var mockEnternalEventQueue = new Mock<IEventQueue>();
        var mockLogManager = new Mock<ILogManager>();
        var inMemorySettings = new Dictionary<string, string?> { };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var quickCache = new QuickCache(
            mockLog.Object,
            mockLogPosition.Object,
            mockLogHash.Object,
            mockInternalEventQueue.Object,
            mockEnternalEventQueue.Object,
            mockLogManager.Object,
            configuration);
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
        var mockInternalEventQueue = new Mock<IEventQueue>();
        var mockEnternalEventQueue = new Mock<IEventQueue>();
        var mockLogManager = new Mock<ILogManager>();
        var inMemorySettings = new Dictionary<string, string?> { };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var quickCache = new QuickCache(
            mockLog.Object,
            mockLogPosition.Object,
            mockLogHash.Object,
            mockInternalEventQueue.Object,
            mockEnternalEventQueue.Object,
            mockLogManager.Object,
            configuration);

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

    [TestMethod]
    public void GetConfigurationValue_KeySet_CallsLogManagerWithValue()
    {
        // Arrange
        var mockLogPosition = new Mock<ILogPosition>();
        var mockLog = new Mock<ILog>();
        var mockLogHash = new Mock<ILogHash>();
        var mockInternalEventQueue = new Mock<IEventQueue>();
        var mockEnternalEventQueue = new Mock<IEventQueue>();
        var mockLogManager = new Mock<ILogManager>();
        mockLogManager.SetupSet(logManager => logManager.LogThreshHold = It.IsAny<int>()).Verifiable();
        var inMemorySettings = new Dictionary<string, string?> {
            {"quick-cache:log-threshold", "5898"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var quickCache = new QuickCache(
            mockLog.Object,
            mockLogPosition.Object,
            mockLogHash.Object,
            mockInternalEventQueue.Object,
            mockEnternalEventQueue.Object,
            mockLogManager.Object,
            configuration);

        // Assert
        mockLogManager.VerifySet(logManager => logManager.LogThreshHold = 5898);
    }

    [TestMethod]
    public void Integration_Set_ShouldRepectThresholdLimit()
    {
        // Arrange
        var threshold = 2;

        var logPosition = new LogPosition();
        var log = new Log();
        var internalEventQueue = new EventQueue();
        var externalEventQueue = new EventQueue();
        var logHash = new LogHash(internalEventQueue);
        var logManager = new LogManager(internalEventQueue, externalEventQueue, log, logHash);

        logManager.LogThreshHold = threshold;

        var inMemorySettings = new Dictionary<string, string?> { };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var quickCache = new QuickCache(
            log,
            logPosition,
            logHash,
            internalEventQueue,
            externalEventQueue,
            logManager,
            configuration);
        var value = 1234;

        // Act
        quickCache.Observable.Subscribe(
            e => { 
                if(e.EventType == EventType.Remove)
                {
                    Assert.AreEqual(2, logHash.GetAllReferences().Count());
                    Assert.AreEqual(2, log.GetAllPositions().Count());
                }
            },
            () =>
            {
            });

        quickCache.Set("Key1", value);
        quickCache.Set("Key2", value);
        quickCache.Set("Key3", value);
    }

    [TestMethod]
    public void Integration_Set_ShouldRemoveKeysCorrectly()
    {
        // Arrange
        var threshold = 2;

        var logPosition = new LogPosition();
        var log = new Log();
        var internalEventQueue = new EventQueue();
        var externalEventQueue = new EventQueue();
        var logHash = new LogHash(internalEventQueue);
        var logManager = new LogManager(internalEventQueue, externalEventQueue, log, logHash);

        logManager.LogThreshHold = threshold;

        var inMemorySettings = new Dictionary<string, string?> { };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var quickCache = new QuickCache(
            log,
            logPosition,
            logHash,
            internalEventQueue,
            externalEventQueue,
            logManager,
            configuration);

        // Act
        quickCache.Observable.Subscribe(
            e => {
                if (e.EventType == EventType.Remove)
                {
                    Assert.AreEqual(2, logHash.GetAllReferences().Count());
                    Assert.AreEqual(2, log.GetAllPositions().Count());
                    Assert.AreEqual(1111, quickCache.Get<int>("Key1"));
                    Assert.AreEqual(2222, quickCache.Get<int>("Key2"));
                    Assert.AreEqual(null, quickCache.Get<int>("Key3"));
                }
            },
            () =>
            {
            });

        quickCache.Set("Key1", 1111);
        quickCache.Set("Key2", 2222);
        quickCache.Set("Key3", 3333);
        quickCache.Delete("Key3");
    }
}