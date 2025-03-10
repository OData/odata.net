namespace V2.Fx.Collections
{
    using System;

    using V2.Fx.Runtime.InteropServices;

    //// TODO double check that this doesn't have any `unsafe` contexts

    /// <summary>
    /// A linked list with nodes that are allocated entirely on the stack and allowing for stack allocated elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public ref struct LinkedList<T> where T : allows ref struct //// TODO what other functionality do you want here? remove? lenght? etc...
    {
        private SpanEx<LinkedListNode> first;

        private SpanEx<LinkedListNode> current;

        private bool hasValues;

        public LinkedList(ByteSpan emptySpan)
        {
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
            //// TODO this method isn't worth it unless you remove the iteration over values

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
