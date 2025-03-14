namespace Fx.Collections
{
    public ref struct LinkedList7<T> where T : allows ref struct
    {
        private BetterSpan<LinkedListNode7> first;

        private BetterSpan<LinkedListNode7> current;

        public LinkedList7(T value, Span<byte> memory)
        {
            var firstNode = new LinkedListNode7(value);
            Unsafe2.Copy(memory, firstNode);

            this.first = BetterSpan.FromMemory<LinkedListNode7>(memory, 1);
            this.current = this.first;
        }

        public void Append(T value, Span<byte> memory)
        {
            var nextNode = new LinkedListNode7(value);
            Unsafe2.Copy(memory, nextNode);

            var next = BetterSpan.FromMemory<LinkedListNode7>(memory, 1);

            this.current.Get(0).Next = next;

            this.current = next;
        }

        public static int MemorySize { get; } = System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode7>();

        internal ref struct LinkedListNode7 //// TODO can you make this private
        {
            public readonly T Value;

            public BetterSpan<LinkedListNode7> Next;

            public LinkedListNode7(T value)
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
            private BetterSpan<LinkedListNode7> node;

            private bool hasMoved;

            internal Enumerator(BetterSpan<LinkedListNode7> node)
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
