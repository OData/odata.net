namespace V2.Fx.Collections
{
    using System;

    using V2.Fx.Runtime.InteropServices;

    //// TODO double check that this doesn't have any `unsafe` contexts

    /// <summary>
    /// A linked list with nodes that are allocated entirely on the stack and allowing for stack allocated elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public ref struct LinkedList<T> where T : allows ref struct
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

        public int MemorySize { get; } = System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode>();

        private ref struct LinkedListNode
        {
            public T Value;

            public SpanEx<LinkedListNode> Next;

            public LinkedListNode(T value)
            {
                this.Value = value;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private SpanEx<LinkedListNode> node;

            private bool hasMoved;

            private readonly bool hasValues;

            internal Enumerator(LinkedList<T> list)
            {
                this.node = list.first;

                this.hasValues = list.hasValues;

                this.hasMoved = false;
            }

            public ref T Current
            {
                get
                {
                    if (!this.hasMoved)
                    {
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    }

                    //// TODO this returns by ref; it's not clear to me that this is the best thing to do; what it allows for is to update elements of the list by setting the element that the caller wants updated as they enumerate
                    return ref this.node[0].Value;
                }
            }

            public bool MoveNext()
            {
                if (!this.hasValues)
                {
                    return false;
                }

                if (!this.hasMoved)
                {
                    this.hasMoved = true;
                    return true;
                }

                var nextNode = this.node[0].Next;
                if (nextNode.Length != 0)
                {
                    this.node = nextNode;
                    return true;
                }

                return false;
            }
        }
    }
}
