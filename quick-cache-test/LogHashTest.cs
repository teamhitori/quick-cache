using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamHitori.QuickCache;

namespace TeamHitori.QuickCacheTest
{
    [TestClass]
    public class LogHashTest
    {
        [TestMethod]
        public void GetLogPosition_ShouldRetrieveCorrectPositionForExistingKey()
        {
            var logHash = new LogHash();
            string key = "testKey";
            ulong expectedPosition = 123; // Example position
            logHash.Set(key, expectedPosition);

            var actualPosition = logHash.GetLogPosition(key);

            Assert.AreEqual(expectedPosition, actualPosition);
        }

        [TestMethod]
        public void GetLogPosition_ShouldReturnNullForNonExistentKey()
        {
            var logHash = new LogHash();
            string nonExistentKey = "nonExistentKey";

            var position = logHash.GetLogPosition(nonExistentKey);

            Assert.IsNull(position);
        }

        [TestMethod]
        public void GetLogPosition_ShouldReturnNullAfterKeyIsRemoved()
        {
            var logHash = new LogHash();
            string key = "testKey";
            logHash.Set(key, 123);
            logHash.RemoveKey(key);

            var position = logHash.GetLogPosition(key);

            Assert.IsNull(position);
        }

        [TestMethod]
        public async Task GetLogPosition_ShouldBeThreadSafeForConcurrentAccess()
        {
            var logHash = new LogHash();
            string key = "testKey";
            ulong expectedPosition = 123;
            logHash.Set(key, expectedPosition);
            var tasks = new List<Task<ulong?>>();

            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Run(() => logHash.GetLogPosition(key)));
            }

            await Task.WhenAll(tasks.ToArray());

            foreach (var task in tasks)
            {
                Assert.AreEqual(expectedPosition, task.Result);
            }
        }

        [TestMethod]
        public void GetLogPosition_ShouldDistinguishSimilarKeys()
        {
            var logHash = new LogHash();
            string key1 = "key";
            string key2 = "key2";
            ulong position1 = 123;
            ulong position2 = 456;
            logHash.Set(key1, position1);
            logHash.Set(key2, position2);

            var retrievedPosition1 = logHash.GetLogPosition(key1);
            var retrievedPosition2 = logHash.GetLogPosition(key2);

            Assert.AreEqual(position1, retrievedPosition1);
            Assert.AreEqual(position2, retrievedPosition2);
        }



    }
}
