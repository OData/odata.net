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

        private string GetResource(string path)
        {
            var assembly = this.GetType().Assembly;
            using (var resourceStream = assembly.GetManifestResourceStream(path)) //// TODO you've previously done something in onboarding for embedded resources, check out that code
            {
                if (resourceStream == null)
                {
                    throw new Exception("TODO");
                }

                using (var textReader = new StreamReader(resourceStream))
                {
                    return textReader.ReadToEnd();
                }
            }
        }
    }
}
