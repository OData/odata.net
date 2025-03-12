namespace Fx.Collections
{
    public ref struct LinkedListNode6<T> where T : allows ref struct
    {
        private readonly BetterSpan<LinkedListNode5<T>> previous;

        private readonly T value;
    }
    public ref struct Data<T> where T : allows ref struct
    {
        public Data(T value, BetterSpan<LinkedListNode5<T>> previous)
        {
            Value = value;
            ////Previous = previous;
        }

        public T Value { get; }
        ////public BetterSpan<LinkedListNode5<T>> Previous { get; }
    }

    public ref struct LinkedListNode5<T> where T : allows ref struct
    {
        private Data<T> data;

        ////private Data<T> container;*/

        public LinkedListNode5(T value)
        {
            this.data = new Data<T>(value, default);
        }

        /*private LinkedListNode5(T value, BetterSpan<LinkedListNode5<T>> previous)
        {
            this.data = new Data<T>()
            {
                value = value,
            };
        }

        public unsafe LinkedListNode5<T> Append(T value)
        {
            var self = this;
            fixed (Container* pointer = &this.container)
            {
                var containerMemory = new Span<byte>(pointer, System.Runtime.CompilerServices.Unsafe.SizeOf<Container>());
                Unsafe2.Copy2(containerMemory, self); //// TODO you need to fix this so that it only actually copies the first two fields

                var span = BetterSpan.FromMemory3<LinkedListNode5<T>>(containerMemory, 1);
                return new LinkedListNode5<T>(value, span);
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private LinkedListNode5<T> node;

            private bool hasMoved;
            
            public Enumerator(LinkedListNode5<T> node)
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

                    return node.data.value;
                }
            }

            public bool MoveNext()
            {
                if (!this.hasMoved)
                {
                    this.hasMoved = true;
                    return true;
                }

                if (this.node.data.previous.Length == 0)
                {
                    return false;
                }


                this.node = node.data.previous[0];
                return true;
            }
        }*/
    }
}
