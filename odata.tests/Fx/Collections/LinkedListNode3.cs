namespace Fx.Collections
{
    public readonly ref struct LinkedListNode3<T> where T : allows ref struct
    {
        private readonly BetterSpan<LinkedListNode3<T>> previous;

        private readonly T values;

        public LinkedListNode3(BetterSpan<T> values, BetterSpan<LinkedListNode3<T>> previous)
        {
            this.values = values[0];
            this.previous = previous;
        }

        public LinkedListNode3<T> Append(BetterSpan<T> values, Span<byte> previousMemory)
        {
            if (previousMemory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode3<T>>())
            {
                throw new Exception("TODO");
            }

            var self = this;
            Unsafe2.Copy(previousMemory, self);

            var span = BetterSpan.FromMemory<LinkedListNode3<T>>(previousMemory, 1);
            return new LinkedListNode3<T>(values, span);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private LinkedListNode3<T> node;

            private bool hasMoved;
            
            public Enumerator(LinkedListNode3<T> node)
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

                    return node.values;
                }
            }

            public bool MoveNext()
            {
                if (!this.hasMoved)
                {
                    this.hasMoved = true;
                    return true;
                }

                if (this.node.previous.Length == 0)
                {
                    return false;
                }


                this.node = node.previous[0];
                return true;
            }
        }
    }
}
