namespace V2.Fx.Collections
{
    using System;

    using V2.Fx.Runtime.CompilerServices;

    public ref struct LinkedList<T> where T : allows ref struct
    {
        private BetterReadOnlySpan<LinkedListNode> first;

        private BetterReadOnlySpan<LinkedListNode> current;

        public LinkedList(T value, Span<byte> memory) //// TODO can you use betterspan instead of span? how about readonlyspan?
        {
            var firstNode = new LinkedListNode(value);
            Unsafe.Copy(memory, firstNode);

            this.first = BetterSpan.FromMemory<LinkedListNode>(memory, 1);
            this.current = this.first;
        }

        public void Append(T value, Span<byte> memory)
        {
            var nextNode = new LinkedListNode(value);
            Unsafe.Copy(memory, nextNode);

            var next = BetterSpan.FromMemory<LinkedListNode>(memory, 1);

            this.current.Get(0).Next = next;

            this.current = next;
        }

        public static int MemorySize { get; } = System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode>();

        internal ref struct LinkedListNode //// TODO can you make this private
        {
            public readonly T Value;

            public BetterReadOnlySpan<LinkedListNode> Next;

            public LinkedListNode(T value)
            {
                this.Value = value;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this.first);
        }

        public ref struct Enumerator
        {
            private BetterReadOnlySpan<LinkedListNode> node;

            private bool hasMoved;

            internal Enumerator(BetterReadOnlySpan<LinkedListNode> node)
            {
                this.node = node;

                this.hasMoved = false;
            }

            public T Current
            {
                get
                {
                    if (!this.hasMoved)
                    {
                        throw new Exception("TODO");
                    }

                    return this.node[0].Value;
                }
            }

            public bool MoveNext()
            {
                if (!this.hasMoved)
                {
                    this.hasMoved = true;
                    return true;
                }

                if (this.node[0].Next.Length != 0)
                {
                    this.node = this.node[0].Next;
                    return true;
                }

                return false;
            }
        }
    }
}
