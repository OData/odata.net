namespace V2.Fx.Collections
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LinkedListMemoryIntegrityTests
    {
        [TestMethod]
        public void SingleValueLinkedListLeavingFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(1, compilationOutput.Length);
            Assert.AreEqual("CS8352", compilationOutput[0].Id);
        }

        [TestMethod]
        public void AppendedListLeavingFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(1, compilationOutput.Length);
            Assert.AreEqual("CS8352", compilationOutput[0].Id);
        }

        [TestMethod]
        public void LoopedAppendedListLeavingFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(1, compilationOutput.Length);
            Assert.AreEqual("CS8352", compilationOutput[0].Id);
        }

        [TestMethod]
        public void ListParameterAppended()
        {
            //// TODO i think this one maybe *should* be able to compile, as long as one where the input parameter is by reference *doesn't* compile
            //// TODO if you do get this compiling, what *shouldn't* compile is appending to the input parameter and then returning the input parameter
            var compilationOutput = Compile();
            Assert.AreEqual(2, compilationOutput.Length);
            CollectionAssert.AreEquivalent(
                new[] { "CS8352", "CS8350" },
                compilationOutput.Select(element => element.Id).ToArray());
        }

        [TestMethod]
        public void ListByReferenceParameterAppended()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(2, compilationOutput.Length);
            CollectionAssert.AreEquivalent(
                new[] { "CS8352", "CS8350" }, 
                compilationOutput.Select(element => element.Id).ToArray());
        }

        [TestMethod]
        public void ListParameterReturned()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(0, compilationOutput.Length);
        }

        [TestMethod]
        public void AppendToDefaultList()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(2, compilationOutput.Length);
            CollectionAssert.AreEquivalent(
                new[] { "CS8352", "CS8350" },
                compilationOutput.Select(element => element.Id).ToArray());
        }

        [TestMethod]
        public void AppendToReturnedDefaultList()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(2, compilationOutput.Length);
            CollectionAssert.AreEquivalent(
                new[] { "CS8352", "CS8350" },
                compilationOutput.Select(element => element.Id).ToArray());
        }

        [TestMethod]
        public void EmptyListLeavingFrame()
        {
            var compilationOutput = Compile();
            Assert.AreEqual(1, compilationOutput.Length);
            Assert.AreEqual("CS8352", compilationOutput[0].Id);
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


        /*public static class V1
        {
            private static LinkedListNode<int> Test3()
            {
        //// TODO this is probably applicable to an empty list
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                return list;
            }


            private static LinkedListNode<int> Test6()
            {
        //// TODO this might apply to the stack implementation
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                Unsafe2.Copy(memory, in list);

                var nextValue = BetterSpan.FromInstance(67);
                var previousNode = BetterSpan.FromMemory<LinkedListNode<int>>(memory, 1);
                list = new LinkedListNode<int>(nextValue, previousNode);

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode<int> Test8()
            {
        //// TODO this might apply to the stack implementation
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                    Unsafe2.Copy(memory, in list);

                    var nextValue = BetterSpan.FromInstance(i);
                    var previousNode = BetterSpan.FromMemory<LinkedListNode<int>>(memory, 1);
                    list = new LinkedListNode<int>(nextValue, previousNode);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode<int> Test15()
            {
        //// TODO this is probably applicable if you allow adding spans of values
                var betterSpan = BetterSpan.FromSpan(new Span<int>(new[] { 1, 2, 3, 4 }));

                var list = new LinkedListNode<int>(betterSpan);

                //// TODO this should be allowed
                return list;
            }
        }*/
    }
}
