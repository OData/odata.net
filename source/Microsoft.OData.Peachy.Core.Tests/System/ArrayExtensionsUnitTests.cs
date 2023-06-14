namespace System
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="ArrayExtensions"/>
    /// </summary>
    [TestClass]
    public class ArrayExtensionsUnitTests
    {
        [TestMethod]
        public void GetNonDefault()
        {
            var data = new[] { "asdf" };
            var element = data.ElementAtOrDefault(0, "default");
            Assert.AreEqual("asdf", element);
        }

        [TestMethod]
        public void GetDefault()
        {
            var data = new[] { "asdf" };
            var element = data.ElementAtOrDefault(1, "default");
            Assert.AreEqual("default", element);
        }
    }
}
