namespace Fx.Collections
{
    public readonly ref struct LinkedListNode<T> where T : allows ref struct
    {
        private readonly BetterSpan<LinkedListNode<T>> previous;

        private readonly BetterSpan<T> values;

        public LinkedListNode(scoped in BetterSpan<T> values)
            : this(values, default)
        {
        }

        internal LinkedListNode(scoped in BetterSpan<T> values, BetterSpan<LinkedListNode<T>> previous) //// TODO make this private
        {
            this.values = values;
            this.previous = previous;
        }

        public LinkedListNode<T> Append(scoped in BetterSpan<T> values, Span<byte> previousMemory) //// TODO do a sweep of `scoped` and `in` to make sure you have the least coverage necessary
        {
            if (previousMemory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode<T>>())
            {
                throw new Exception("TODO");
            }

            var self = this;
            Unsafe2.Copy(previousMemory, in self);

            return new LinkedListNode<T>(values, BetterSpan.FromMemory<LinkedListNode<T>>(previousMemory, 1));
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private LinkedListNode<T> node;

            private int index;

            public Enumerator(LinkedListNode<T> node)
            {
                this.node = node;

                this.index = -1;
            }

            public T Current
            {
                get
                {
                    if (this.index < 0)
                    {
                        throw new Exception("TODO");
                    }

                    return node.values[this.index];
                }
            }

            public bool MoveNext()
            {
                if (this.index < 0)
                {
                    if (this.node.values.Length > 0)
                    {
                        this.index = 0;
                        return true;
                    }

                    if (node.previous.Length == 0)
                    {
                        return false;
                    }

                    this.node = node.previous[0];
                    return this.MoveNext();
                }

                if (this.index < this.node.values.Length - 1)
                {
                    ++this.index;
                    return true;
                }

                if (node.previous.Length == 0)
                {
                    return false;
                }

                this.node = node.previous[0];
                return this.MoveNext();
            }
        }
    }
}
