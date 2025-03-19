namespace V2.Fx
{
    using System.Runtime.CompilerServices;

    using Microsoft.VisualStudio.TestTools.UnitTesting;    

    [TestClass]
    public sealed class BetterReadOnlySpanMemoryIntegrityTests
    {
        [TestMethod]
        public void StackAllocWithinFrame()
        {
            Span<byte> span = stackalloc byte[Unsafe.SizeOf<string>()];
            var betterSpan = BetterReadOnlySpan.FromMemory<string>(span, 1);
        }
    }
}
