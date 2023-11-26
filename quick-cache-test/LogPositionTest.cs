using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamHitori.QuickCacheTest
{
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
                values.Add(logPosition.GetNewPosition().Value);
            }

            // Assert
            Assert.AreEqual(1000, values.Count);
        }

        [TestMethod]
        public void GetNewPosition_ShouldIncrementSequentially()
        {
            // Arrange
            var logPosition = new LogPosition();
            ulong previous = logPosition.GetNewPosition().Value;

            // Act
            // Assert
            for (int i = 0; i < 100; i++)
            {
                ulong current = logPosition.GetNewPosition().Value;
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
                var logPos = logPosition.GetNewPosition().Value;
                values.Add(logPos);
                queue.Enqueue(logPos);
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
}
