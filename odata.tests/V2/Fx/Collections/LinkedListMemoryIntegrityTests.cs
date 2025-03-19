namespace V2.Fx.Collections
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

        private ImmutableArray<Diagnostic> Compile([CallerMemberName] string? testMethod = null)
        {
            var scriptContents = GetResource(testMethod);
            var script = CSharpScript
                .Create(
                    scriptContents,
                    ScriptOptions
                        .Default
                        .WithReferences(
                            typeof(BetterReadOnlySpan<>).Assembly));

            return script.Compile();
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

            private static LinkedListNode<int> Test9()
            {
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                    list = list.Append(BetterSpan.FromInstance(i), memory);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static void Test10(LinkedListNode<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];

                //// THIS IS A GOOD THING
                list = list.Append(BetterSpan.FromInstance(42), memory);
            }

            private static LinkedListNode<int> Test11(LinkedListNode<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];

                //// THIS IS A GOOD THING
                return list.Append(BetterSpan.FromInstance(42), memory);
            }

            private static LinkedListNode<int> Test12(LinkedListNode<int> list)
            {
                return list;
            }

            private static LinkedListNode<int> Test13()
            {
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                    list = list.Append(i, memory);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode<int> Test14()
            {
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));

                //// TODO this should be allowed
                return list;
            }

            private static LinkedListNode<int> Test15()
            {
                var betterSpan = BetterSpan.FromSpan(new Span<int>(new[] { 1, 2, 3, 4 }));

                var list = new LinkedListNode<int>(betterSpan);

                //// TODO this should be allowed
                return list;
            }
        }*/
        /*public static class V2
        {
            private static LinkedListNode2<int> Test8()
            {
                var list = new LinkedListNode2<int>(42);
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                    Unsafe2.Copy(memory, in list);

                    var nextValue = i;
                    var previousNode = BetterSpan.FromMemory<LinkedListNode2<int>>(memory, 1);
                    list = new LinkedListNode2<int>(nextValue, previousNode);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test9()
            {
                //// TODO look at this!

                var list = new LinkedListNode2<int>(42);
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                LinkedListNode2<int> list2 = list.Append(0, memory);
                for (int i = 1; i < 10; ++i)
                {
                    Span<byte> memory2 = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                    list2 = list2.Append(i, memory2);
                }

                //// THIS IS A GOOD THING
                return list2;
            }

            private static void Test10(LinkedListNode2<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];

                //// THIS IS A GOOD THING
                list = list.Append(42, memory);
            }

            private static LinkedListNode2<int> Test11(LinkedListNode2<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];

                //// THIS IS A GOOD THING
                return list.Append(42, memory);
            }

            private static LinkedListNode2<int> Test12(LinkedListNode2<int> list)
            {
                return list;
            }

            private static LinkedListNode2<int> Test13()
            {
                var list = new LinkedListNode2<int>(42);
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                    list = list.Append(i, memory);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test14()
            {
                var list = new LinkedListNode2<int>(42);

                //// TODO this should be allowed
                //// this doesn't work currently because `linkedlistnode2.previous` will get initialized to its default, and since it has a `T*` field, it might have a pointer to somewhere in this stackframe, and so when we return `list`, the first node in the list (in this case, the only node) will have a reference to a pointer to the stackframe that has already been popped off; of course, *we* know that the pointer in this case will be `0`, but the compiler doesn't have a way to know that
                //// TODO i don't think the above is true, i think it's because the constructor takes in a `in` parameter, so they are receiving `42` *by reference to its location in the stackframe*, so when we return, that referenced value is gone
                return list;
            }
        }*/
    }
}
