namespace quick_cache_test
{
    [TestClass]
    public class QuickCacheTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var quickCache = new QuickCache();
            var result = quickCache.Get();
            Assert.AreEqual("Hello World", result);
        }
    }


}