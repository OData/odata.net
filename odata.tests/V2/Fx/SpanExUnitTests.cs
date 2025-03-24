namespace V2.Fx
{
    using System;

    [TestClass]
    public sealed class SpanExUnitTests
    {
        [TestMethod]
        public void FromInstance()
        {
            var value = 42;
            var span = SpanEx.FromInstance(ref value);

            Span<byte> zeroed = stackalloc byte[100];
            zeroed.Clear();

            Assert.AreEqual(42, span[0]);
        }
    }
}
