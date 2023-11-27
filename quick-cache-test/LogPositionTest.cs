
using System.Collections.Concurrent;

[TestClass]
public class LogPositionTest
{
    [TestMethod]
    public void GetNewPosition_ShouldGenerateUniqueValues()
    {
        // Arrange
        var logPosition = new LogPosition();
        var values = new HashSet<ulong>();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            var newPosition = logPosition.GetNewPosition();
            if (newPosition != null)
                values.Add(newPosition.Value);
        }

        // Assert
        Assert.AreEqual(1000, values.Count);
    }

    [TestMethod]
    public void GetNewPosition_ShouldIncrementSequentially()
    {
        // Arrange
        var logPosition = new LogPosition();
        var previous = logPosition.GetNewPosition();

        // Act
        // Assert
        for (int i = 0; i < 100; i++)
        {
            var current = logPosition.GetNewPosition();
            Assert.AreEqual(previous + 1, current);
            previous = current;
        }
    }

    [TestMethod]
    public void GetNewPosition_ShouldBeThreadSafe()
    {
        // Arrange
        var logPosition = new LogPosition();
        var values = new ConcurrentBag<ulong>();
        var queue = new ConcurrentQueue<ulong?>();

        // Act
        Parallel.For(0, 1000, (i) =>
        {
            var logPos = logPosition.GetNewPosition();
            if (logPos != null)
            {
                values.Add(logPos.Value);
                queue.Enqueue(logPos);
            }
            
        });

        // Assert
        Assert.AreEqual(1000, values.Distinct().Count());
    }

    [TestMethod]
    public void GetNewPosition_ShouldStartFromExpectedInitialValue()
    {
        // Arrange
        var logPosition = new LogPosition();
        ulong expectedInitialValue = 1;

        // Act
        // Assert
        Assert.AreEqual(expectedInitialValue, logPosition.GetNewPosition());
    }

    [TestMethod]
    public void GetNewPosition_ShouldHandleBoundaryCondition()
    {
        // Arrange
        var logPosition = new LogPosition(ulong.MaxValue);

        // Act
        // Assert
        Assert.AreEqual(null, logPosition.GetNewPosition());
    }
}
