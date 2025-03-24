namespace V2.Fx
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;    

    [TestClass]
    public sealed class SpanExFactoryMemoryIntegrityTests
    {
        [TestMethod]
        public void StackAllocWithinFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(0, compilationOutput.Length);
        }

        [TestMethod]
        public void StackAllocLeavingFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(1, compilationOutput.Length);
            Assert.AreEqual("CS8352", compilationOutput[0].Id);
        }

        [TestMethod]
        public void CopiedMemoryLeavingFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(1, compilationOutput.Length);
            Assert.AreEqual("CS8352", compilationOutput[0].Id);
        }

        [TestMethod]
        public void MemoryCopiedToCallingFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(0, compilationOutput.Length);
        }

        [TestMethod]
        public void WrappedValueLeavingFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(0, compilationOutput.Length);
        }

        private ImmutableArray<Diagnostic> Compile([CallerMemberName] string? testMethod = null)
        {
            var scriptContents = GetResource(testMethod);
            var script = CSharpScript
                .Create(
                    scriptContents,
                    ScriptOptions
                        .Default
                        .WithReferences(
                            typeof(SpanEx<>).Assembly));

            return script.Compile();
        }

        private string GetResource([CallerMemberName] string? testMethod = null)
        {
            ArgumentNullException.ThrowIfNull(testMethod);

            var type = this.GetType();
            var path = $"{type.Namespace}.{type.Name}Resources.{testMethod}.cs";
            var assembly = type.Assembly;
            var names = assembly.GetManifestResourceNames();
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
