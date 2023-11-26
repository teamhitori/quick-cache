using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamHitori.QuickCache;

namespace TeamHitori.QuickCacheTest
{
    [TestClass]
    public class LogPositionTest
    {
        [TestMethod]
        public void GetNewPosition_ShouldGenerateUniqueValues()
        {
            var logPosition = new LogPosition();
            var values = new HashSet<ulong>();

            for (int i = 0; i < 1000; i++)
            {
                values.Add(logPosition.GetNewPosition().Value);
            }

            Assert.AreEqual(1000, values.Count);
        }

        [TestMethod]
        public void GetNewPosition_ShouldIncrementSequentially()
        {
            var logPosition = new LogPosition();
            ulong previous = logPosition.GetNewPosition().Value;

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
            var logPosition = new LogPosition();
            var values = new ConcurrentBag<ulong>();
            var queue = new ConcurrentQueue<ulong?>();

            Parallel.For(0, 1000, (i) =>
            {
                var logPos = logPosition.GetNewPosition().Value;
                values.Add(logPos);
                queue.Enqueue(logPos);
            });

            var distinctValues = string.Join(",", queue.Order());

            var res = queue.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => new { Element = y.Key, Counter = y.Count() })
              .ToList();

            Assert.AreEqual(1000, values.Distinct().Count());
        }

        [TestMethod]
        public void GetNewPosition_ShouldStartFromExpectedInitialValue()
        {
            var logPosition = new LogPosition();
            ulong expectedInitialValue = 1; // or 1, based on your implementation
            Assert.AreEqual(expectedInitialValue, logPosition.GetNewPosition());
        }

        [TestMethod]
        public void GetNewPosition_ShouldHandleBoundaryCondition()
        {
            var logPosition = new LogPosition(ulong.MaxValue);

            Assert.AreEqual(null, logPosition.GetNewPosition());
        }
    }
}
