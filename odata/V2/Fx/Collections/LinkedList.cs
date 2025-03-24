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

        public LinkedList()
        {
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
        public LinkedList(T value, ByteSpan memory)
        {
            //// TODO do you still want this constructor now that empty lists are a thing?
            this.SetFirstValue(value, memory);
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

        public void Append(T value, ByteSpan memory)
        {
            if (!this.hasValues)
            {
                this.SetFirstValue(value, memory);
            }
            else
            {
                var nextNode = new LinkedListNode(value);
                MemoryMarshal.Write(memory, nextNode);

                var next = SpanEx.FromMemory<LinkedListNode>(memory, 1);

                this.current[0].Next = next;

                this.current = next;
            }
        }

        public static int MemorySize { get; } = System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode>();

        private ref struct LinkedListNode
        {
            public readonly T Value;

            public SpanEx<LinkedListNode> Next;

            public LinkedListNode(T value)
            {
                this.Value = value;
            }
        }

        public Enumerator GetEnumerator()
        {
            if (this.hasValues)
            {
                return new Enumerator(this);
            }
            else
            {
                return new Enumerator();
            }
        }

        public ref struct Enumerator
        {
            private SpanEx<LinkedListNode> node;

            private bool hasMoved;

            private readonly bool hasValues;

            internal Enumerator(LinkedList<T> list)
            {
                this.node = list.first;

                this.hasMoved = false;
                this.hasValues = true;
            }

            public T Current //// TODO cna you make this return `ref t`?
            {
                get
                {
                    if (!this.hasValues || !this.hasMoved)
                    {
                        throw new Exception("TODO");
                    }

                    return this.node[0].Value;
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
