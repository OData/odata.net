namespace Fx.Collections
{
    public readonly ref struct LinkedListNode2<T> where T : allows ref struct
    {
        private readonly BetterSpan<LinkedListNode2<T>> previous;

        private readonly T value;

        public LinkedListNode2(in T value)
        {
            this.value = value;
        }

        internal LinkedListNode2(T value, BetterSpan<LinkedListNode2<T>> previous) //// TODO make this private
        {
            this.value = value;
            this.previous = previous;
        }

        public LinkedListNode2<T> Append(T value, Span<byte> previousMemory) //// TODO do a sweep of `scoped` and `in` to make sure you have the least coverage necessary
        {
            if (previousMemory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode2<T>>())
            {
                throw new Exception("TODO");
            }

            var self = this;
            Unsafe2.Copy(previousMemory, self);

            var span = BetterSpan.FromMemory<LinkedListNode2<T>>(previousMemory, 1);
            return new LinkedListNode2<T>(value, span);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private LinkedListNode2<T> node;

            private bool hasMoved;
            
            public Enumerator(LinkedListNode2<T> node)
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

                    return node.value;
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
