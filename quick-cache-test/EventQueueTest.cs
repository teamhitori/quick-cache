using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamHitori.QuickCacheTest
{
    [TestClass]
    public class EventQueueTest
    {
        [TestMethod]
        public void RaiseEvent_ShouldEnqueueEventSuccessfully()
        {
            var eventQueue = new EventQueue();
            var eventType = EventType.Add; 
            var key = "someKey";
            Event raisedEvent = null;

            eventQueue.Observe().Subscribe(e => raisedEvent = e);

            eventQueue.RaiseEvent(eventType, key);

            Thread.Sleep(100);
            
            Assert.AreEqual(eventType, raisedEvent.EventType);
            Assert.AreEqual(key, raisedEvent.Key);
        }

        [TestMethod]
        public void RaiseEvent_ShouldPreserveOrderOfEvents()
        {
            var eventQueue = new EventQueue();
            var firstEvent = EventType.Add; 
            var secondEvent = EventType.Remove;

            eventQueue.Observe()
                .Select((x, i) => (x, i))
                .Subscribe(e => Assert.AreEqual($"Key:{e.i}", $"{e.x.Key}"));

            eventQueue.RaiseEvent(firstEvent, "Key:1");
            eventQueue.RaiseEvent(secondEvent, "Key:2");
            eventQueue.RaiseEvent(firstEvent, "Key:3");
            eventQueue.RaiseEvent(secondEvent, "Key:4");

            Thread.Sleep(100);
        }

        [TestMethod]
        public void RaiseEvent_ShouldBeThreadSafeForConcurrentCalls()
        {
            var eventQueue = new EventQueue();
            var tasks = new List<Task>();
            var count = 0;

            eventQueue.Observe()
                .Subscribe(e => Interlocked.Increment(ref count));

            for (int i = 0; i < 1000; i++)
            {
                tasks.Add(Task.Run(() => eventQueue.RaiseEvent(EventType.Add, $"key:{i}")));
            }

            Task.WaitAll(tasks.ToArray());

            Thread.Sleep(100);

            Assert.AreEqual(1000, count);
        }
    }
}
