using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TeamHitori.QuickCacheTest
{
    [TestClass]
    public class LogTest
    {
        [TestMethod]
        public void AddValue_ShouldAddValueSuccessfully()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();
            var position = logPosition.GetNewPosition();
            int value = 1234;

            // Act
            log.AddValue(position!.Value, value);
            var result = log.GetValue<int>(position!.Value);

            // Assert
            Assert.AreEqual(value, result!.Value);
        }

        [TestMethod]
        public void AddValue_ShouldHandleDuplicatePositions()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();
            var position = logPosition.GetNewPosition();
            var value1 = new MyStruct() { Arr = [1, 2, 3] };
            var value2 = new MyStruct() { Arr = [4, 5, 6] }; ;

            // Act
            log.AddValue(position!.Value, value1);

            // Assert
            Assert.ThrowsException<InvalidOperationException>(() => log.AddValue(position!.Value, value2));
        }

        [TestMethod]
        public async Task AddValue_ShouldBeThreadSafeForConcurrentAdditions()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 1000; i++)
            {
                var value = new MyStruct() { Arr = new int[1024] };
                tasks.Add(Task.Run(() => log.AddValue(logPosition.GetNewPosition()!.Value, value)));
            }

            await Task.WhenAll(tasks.ToArray());

            var values = log.GetAllPositions();

            // Assert
            Assert.AreEqual(1000, values.Distinct().Count());
        }

        [TestMethod]
        public void GetValue_ShouldReturnNullForNonExistentPosition()
        {
            // Arrange
            var log = new Log();
            ulong nonExistentPosition = 9999;

            // Act
            var result = log.GetValue<int>(nonExistentPosition);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetValue_ShouldBeThreadSafeForConcurrentReads()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();
            var value = 1234;
            var position = logPosition.GetNewPosition();
            log.AddValue(position!.Value, value);
            var tasks = new List<Task<int>>();

            // Act
            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Run(() => log.GetValue<int>(position.Value)!.Value));
            }

            await Task.WhenAll(tasks.ToArray());

            // Assert
            foreach (var task in tasks)
            {
                Assert.AreEqual(value, task.Result);
            }
        }

        [TestMethod]
        public void GetValue_AfterDeletion_ShouldReturnNull()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();
            var position = logPosition.GetNewPosition();
            var value = 1234;
            log.AddValue(position!.Value, value);
            log.RemoveValue(position!.Value);

            // Act
            var result = log.GetValue<int>(position!.Value);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Reset_ShouldClearAllItems()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();

            // Act
            log.AddValue(logPosition.GetNewPosition()!.Value, 1234);
            log.AddValue(logPosition.GetNewPosition()!.Value, 5678);

            log.Clear();

            // Assert
            var allValues = log.GetAllPositions();
            Assert.AreEqual(0, allValues.Count());
        }

        [TestMethod]
        public void Reset_OnEmptyLog_ShouldNotCauseIssues()
        {
            // Arrange
            var log = new Log();

            // Act
            log.Clear();

            // Assert
            // If we get here, then the test passed
        }

        [TestMethod]
        public void Reset_FollowedByAddValue_ShouldAcceptNewEntries()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();

            // Act
            log.AddValue(logPosition.GetNewPosition()!.Value, 1234);
            log.Clear();
            var newPosition = logPosition.GetNewPosition();
            var newValue = 5678;
            log.AddValue(newPosition!.Value, newValue);

            // Assert
            var result = log.GetValue<int>(newPosition!.Value);
            Assert.AreEqual(newValue, result);
        }

        [TestMethod]
        public void RemoveValue_ShouldSuccessfullyRemoveExistingValue()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();
            var position = logPosition.GetNewPosition()!.Value;
            var value = 1234;

            // Act
            log.AddValue(position, value);
            log.RemoveValue(position);
            var result = log.GetValue<int>(position);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveValue_ShouldHandleNonExistentPosition()
        {
            // Arrange
            var log = new Log();
            ulong nonExistentPosition = 9999;

            // Act
            log.RemoveValue(nonExistentPosition);

            // Assert
            // If we get here, then the test passed
        }

        [TestMethod]
        public async Task RemoveValue_ShouldBeThreadSafeForConcurrentRemovals()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();
            
            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                var position = logPosition.GetNewPosition()!.Value;
                tasks.Add(Task.Run(() => log.AddValue(position, i)));
            }
            await Task.WhenAll(tasks.ToArray());
            ;
            
            var positions = log.GetAllPositions();

            foreach (var position in positions)
            {
                tasks.Add(Task.Run(() => log.RemoveValue(position)));
            }

            await Task.WhenAll(tasks.ToArray());

            // Assert
            Assert.AreEqual(0, log.GetAllPositions().Count());
        }

        [TestMethod]
        public void GetAllPositions_ShouldRetrieveAllPositions()
        {
            // Arrange
            var log = new Log();
            var logPosition = new LogPosition();
            var positions = new List<ulong>();

            // Act
            for (int i = 0; i < 5; i++)
            {
                var position = logPosition.GetNewPosition()!.Value;
                positions.Add(position);
                log.AddValue(position, 1);
            }

            var retrievedPositions = log.GetAllPositions();

            // Assert
            CollectionAssert.AreEquivalent(positions, retrievedPositions);
        }

        [TestMethod]
        public void GetAllPositions_ShouldReturnEmptyForEmptyLog()
        {
            // Arrange
            var log = new Log();

            // Act
            var positions = log.GetAllPositions();

            // Assert
            Assert.AreEqual(0, positions.Count());
        }

        [TestMethod]
        public void GetAllPositions_ShouldUpdateAfterRemovingValues()
        {
            var log = new Log();
            var logPosition = new LogPosition();
            var position1 = logPosition.GetNewPosition()!.Value;
            var position2 = logPosition.GetNewPosition()!.Value;
            log.AddValue(position1, 1234);
            log.AddValue(position2, 5678);

            log.RemoveValue(position1);
            var positions = log.GetAllPositions();

            Assert.AreEqual(1, positions.Length);
            Assert.AreEqual(position2, positions[0]);
        }

        [TestMethod]
        public void GetAllPositions_ShouldReturnEmptyAfterReset()
        {
            var log = new Log();
            var logPosition = new LogPosition();
            log.AddValue(logPosition.GetNewPosition()!.Value, 1234);

            log.Clear();
            var positions = log.GetAllPositions();

            Assert.AreEqual(0, positions.Count());
        }



        record struct MyStruct
        {
            public int[] Arr;
        }

    }
}
