namespace Fx.Collections
{
    public readonly ref struct LinkedListNode4<T> where T : allows ref struct
    {
        private readonly BetterSpan2<LinkedListNode4<T>> previous;

        private readonly BetterSpan2<T> values;

        public LinkedListNode4(BetterSpan2<T> values, BetterSpan2<LinkedListNode4<T>> previous)
        {
            this.values = values;
            this.previous = previous;
        }

        public LinkedListNode4<T> Append(BetterSpan2<T> values, Span<byte> previousMemory)
        {
            if (previousMemory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode4<T>>())
            {
                throw new Exception("TODO");
            }

            var self = this;
            Unsafe2.Copy(previousMemory, self);

            var span = BetterSpan.FromMemory2<LinkedListNode4<T>>(previousMemory, 1);
            return new LinkedListNode4<T>(values, span);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private LinkedListNode4<T> node;

            private bool hasMoved;
            
            public Enumerator(LinkedListNode4<T> node)
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

                    return node.values[0]; //// TODO go through all of the values in the node
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
