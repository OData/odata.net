namespace System
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Failure tests for <see cref="ArrayExtensions"/>
    /// </summary>
    [TestClass]
    public class ArrayExtensionsFailureTests
    {
        [TestMethod]
        public void GetNonDefault()
        {
            string[]? data = null;
            Assert.ThrowsException<ArgumentNullException>(() => data.ElementAtOrDefault(0, "default"));
        }

        [TestMethod]
        public void GetDefault()
        {
            var data = new[] { "asdf" };
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => data.ElementAtOrDefault(-1, "default"));
        }
    }
}
