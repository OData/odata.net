namespace Fx.Collections
{
    using System.Runtime.CompilerServices;

    public ref struct LinkedListNode6<T> where T : allows ref struct
    {
        private readonly BetterSpan<LinkedListNode5<T>> previous;

        private readonly T value;
    }

    public ref struct LinkedListNode5<T> where T : allows ref struct
    {
        private BetterSpan<LinkedListNode5<T>> previous;

        private T value;

        private Container container;

        private ref struct Container
        {
            byte byte1;
            byte byte2;
            byte byte3;
            byte byte4;
            byte byte5;
            byte byte6;
            byte byte7;
            byte byte8;
            byte byte9;
            byte byte10;
            byte byte11;
            byte byte12;
            byte byte13;
            byte byte14;
            byte byte15;
            byte byte16;
            byte byte17;
            byte byte18;
            byte byte19;
            byte byte20;
            byte byte21;
            byte byte22;
            byte byte23;
            byte byte24;
            byte byte25;
            byte byte26;
            byte byte27;
            byte byte28;
            byte byte29;
            byte byte30;
            byte byte31;
            byte byte32;
        }

        public LinkedListNode5(T value)
        {
            this.value = value;
            this.previous = default;
        }

        public unsafe LinkedListNode5<T> Append(T value)
        {
            var self = this;
            var next = new LinkedListNode5<T>(value);

            Span<byte> nextContainerMemory = new Span<byte>(&next.container, Unsafe.SizeOf<Container>());
            var previousMemory = nextContainerMemory.Slice(0, Unsafe.SizeOf<BetterSpan<LinkedListNode5<T>>>());
            Unsafe2.Copy2(previousMemory, self.previous);
            var valueMemory = nextContainerMemory.Slice(Unsafe.SizeOf<BetterSpan<LinkedListNode5<T>>>(), Unsafe.SizeOf<T>() + 4); //// TODO why + 4?
            Unsafe2.Copy2(valueMemory, self.value);

            next.previous = new BetterSpan<LinkedListNode5<T>>(nextContainerMemory, 1, false);

            return next;

            /*fixed (Container* pointer = &next.container)
            {
                var containerMemory = new Span<byte>(pointer, System.Runtime.CompilerServices.Unsafe.SizeOf<Container>());
                Unsafe2.Copy2(containerMemory, self); //// TODO you need to fix this so that it only actually copies the first two fields

                var span = BetterSpan.FromMemory3<LinkedListNode5<T>>(containerMemory, 1);
                return new LinkedListNode5<T>(value, span);
            }*/
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
