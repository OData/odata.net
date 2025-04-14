namespace V2.Fx.Collections
{
    using System;
    using V2.Fx.Runtime.InteropServices;

    //// TODO double check that this doesn't have any `unsafe` contexts

    /// <summary>
    /// A linked list with nodes that are allocated entirely on the stack and allowing for stack allocated elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public ref struct LinkedList<T> where T : struct, allows ref struct //// TODO what other functionality do you want here? remove? lenght? etc...
    {
        private SpanEx<LinkedListNode> first;

        private SpanEx<LinkedListNode> current;

        private bool hasValues;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emptySpan"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <remarks>
        /// An issue occurs when <typeparamref name="T"/> is a reference type or has members that are reference types. Since the <see cref="LinkedListNode"/>s are stored in <see cref="SpanEx"/>s, which are effectively <see cref="Span{byte}"/> underneath, the garbage collector loses references to the <see cref="LinkedListNode"/>, and therefore loses the reference to the <typeparamref name="T"/> instance. As a result, the <typeparamref name="T"/> instance and any reference it may have as a member are subject to garbage collection. This can be observed in the <see cref="LinkedListUnitTests.ReferenceElements"/> unit test by removing the below <see cref="System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences{T}"/> check and by removing the <see cref="Assert.ThrowsException"/> in the test case. The initialized instances of <see cref="ReferencedElementWithFinalizer"/> have no references to them, and so their finalizers are called, incrementing <see cref="ReferenceElementsFinalized"/>.
        /// 
        /// To mitigate this, only types that are entirely stack allocated are allowed in this list, and this requirement is asserted in this method. 
        /// 
        /// <see cref="System.Runtime.InteropServices.GCHandle"/> was explored as a possible option. However, there are a couple of issues:
        /// 1. It only keeps handles to `object`. This means that, if <typeparamref name="T"/> is a <see langword="struct"/>, a boxing will occur. Further, it doesn't handle the situation where <typeparamref name="T"/> is a <see langword="ref"/> <see langword="struct"/> where boxing is not possible. The <see langword="ref"/> <see langword="struct"/> may contain members which are heap-based, and so losing track of the <see langword="ref"/> <see langword="struct"/> means that its members may be garbage collected.
        /// 2. The <see cref="System.Runtime.InteropServices.GCHandle"/> instance must be cleaned up, so <see cref="LinkedList{T}"/> would need to implement <see cref="IDisposable"/> and call <see cref="System.Runtime.InteropServices.GCHandle.Free"/> on each allocated handle. This would mean that deallocating the list would require traversing the list, somewhat counteracting the benefits of a stack-allocated linked list.
        /// </remarks>
        public LinkedList(ByteSpan emptySpan)
        {
            //// TODO you need to do a full code review of this and its dependencies before using this type
            //// TODO create a work item to explore the toarray stuff in v3/enumerationtests
            //// TODO create a work item to explore the "multiple node allocation" stuff in v3/allocationplayground
            //// TODO create a work item to explore the below "transmutation" stuff

            if (System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                //// TODO explore this as a possible option: https://blog.adamfurmanek.pl/2016/05/07/custom-memory-allocation-in-c-part-3/
                //// TODO this might be useful reference material: https://blog.tchatzigiannakis.com/changing-an-objects-type-at-runtime-in-c-sharp/
                throw new Exception("TODO");
            }

            if (emptySpan.Length != 0)
            {
                // this is to protect the caller from overallocating memory; we don't use this span for anything, it only exists
                // for compiler checks on memory integrity
                var message = $"A new list must be instantiated with empty data. The provided data has length '{emptySpan.Length}'";
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            this.hasValues = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="memory"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <see cref="ByteSpan.Length"/> of <paramref name="memory"/> is not the same as the
        /// <see langword="sizeof"/> <typeparamref name="T"/>
        /// </exception>
        private void SetFirstValue(T value, ByteSpan memory)
        {
            var firstNode = new LinkedListNode(value);
            MemoryMarshal.Write(memory, firstNode);

            this.first = SpanEx.FromMemory<LinkedListNode>(memory, 1);
            this.current = this.first;

            this.hasValues = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="memory"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <see cref="ByteSpan.Length"/> of <paramref name="memory"/> is not the same as the
        /// <see langword="sizeof"/> <typeparamref name="T"/>
        /// </exception>
        public void Append(T value, ByteSpan memory)
        {
            if (this.hasValues)
            {
                var nextNode = new LinkedListNode(value);
                MemoryMarshal.Write(memory, nextNode);

                var next = SpanEx.FromMemory<LinkedListNode>(memory, 1);

                this.current[0].Next = next;

                this.current = next;
            }
            else
            {
                this.SetFirstValue(value, memory);
            }
        }

        private void SetFirstValue(SpanEx<T> values, ByteSpan memory)
        {
            var firstNode = new LinkedListNode(values);
            MemoryMarshal.Write(memory, firstNode);

            this.first = SpanEx.FromMemory<LinkedListNode>(memory, 1);
            this.current = this.first;

            this.hasValues = true;
        }

        public void Append(SpanEx<T> values, ByteSpan memory)
        {
            if (!this.hasValues)
            {
                this.SetFirstValue(values, memory);
            }
            else
            {
                var nextNode = new LinkedListNode(values);
                MemoryMarshal.Write(memory, nextNode);

                var next = SpanEx.FromMemory<LinkedListNode>(memory, 1);

                this.current[0].Next = next;

                this.current = next;
            }
        }

        public int MemorySize { get; } = System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode>();

        private ref struct LinkedListNode
        {
            public T Value;
            public SpanEx<T> Values;

            public SpanEx<LinkedListNode> Next;

            /// <summary>
            /// I decided to have 2 "variants" of this structure. Both could "reduce" to the <see cref="SpanEx{T}"/> variant.
            /// However, doing this would mean that the <see cref="LinkedList{T}.Append(T, ByteSpan)"/> method would need to
            /// take a <see langword="ref"/> parameter (because otherwise the memory that holds the <see cref="SpanEx{T}"/> would
            /// be in the <see cref="LinkedList{T}.Append(T, ByteSpan)"/> stack frame and not in the caller stack frame). Since
            /// a <see langword="ref"/> parameter makes it difficult for callers to append simple values (for example, 
            /// `list.Append(42, stackalloc byte[list.MemorySize])` needs to be `var value = 42; 
            /// list.Append(ref value, stackalloc byte[list.MemorySize]), I made the decision to have different kinds of nodes
            /// that use this property to differentiate between the two.
            /// </summary>
            public bool IsSpan;

            public LinkedListNode(T value)
            {
                this.Value = value;
                this.IsSpan = false;
            }

            public LinkedListNode(SpanEx<T> values)
            {
                this.Values = values;
                this.IsSpan = true;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private SpanEx<LinkedListNode> node;

            private int index;

            private bool hasMoved;

            private readonly bool hasValues;

            internal Enumerator(LinkedList<T> list)
            {
                this.node = list.first;

                this.hasValues = list.hasValues;

                this.hasMoved = false;

                this.index = -1;
            }

            public ref T Current
            {
                get
                {
                    if (this.index == -1)
                    {
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    }

                    //// TODO this returns by ref; it's not clear to me that this is the best thing to do; what it allows for is to update elements of the list by setting the element that the caller wants updated as they enumerate
                    if (this.node[0].IsSpan)
                    {
                        return ref this.node[0].Values[this.index];
                    }
                    else
                    {
                        return ref this.node[0].Value;
                    }
                }
            }

            public bool MoveNext()
            {
                if (!this.hasValues)
                {
                    return false;
                }

                if (this.index == -1)
                {
                    this.index = 0;
                    return true;
                }

                if (this.node[0].IsSpan)
                {
                    if (this.index < this.node[0].Values.Length - 1)
                    {
                        ++this.index;
                        return true;
                    }
                }

                var nextNode = this.node[0].Next;
                if (nextNode.Length != 0)
                {
                    this.node = nextNode;
                    this.index = 0;
                    return true;
                }

                return false;
            }
        }
    }
}
