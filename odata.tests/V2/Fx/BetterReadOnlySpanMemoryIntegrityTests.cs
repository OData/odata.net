namespace V2.Fx
{
    using System.Runtime.CompilerServices;

    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;    

    [TestClass]
    public sealed class BetterReadOnlySpanMemoryIntegrityTests
    {
        [TestMethod]
        public void StackAllocWithinFrame()
        {
            var scriptContents = GetResource();
            var script = CSharpScript
                .Create(
                    scriptContents,
                    ScriptOptions
                        .Default
                        .WithReferences(typeof(BetterReadOnlySpan<>).Assembly)
                        .WithReferences(typeof(Span<>).Assembly));

            var output = script.Compile();
        }

        private string GetResource([CallerMemberName] string? testMethod = null)
        {
            //// TODO if the caller explicitly provides `null`, what happens?

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
