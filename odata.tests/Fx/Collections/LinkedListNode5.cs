namespace Fx.Collections
{
    using System.Runtime.CompilerServices;

    public ref struct LinkedListNode6<T> where T : allows ref struct
    {
        private readonly BetterSpan<LinkedListNode5<T>> previous;

        private readonly T value;
    }

    public unsafe ref struct LinkedListNode5<T> where T : allows ref struct
    {
        private LinkedListNode5<T>* previous;

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

        private static void Copy(byte* source, byte* destination, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                destination[i] = source[i];
            }
        }

        public LinkedListNode5<T> Append(T value)
        {
            fixed (LinkedListNode5<T>* thisPointer = &this)
            {
                var next = new LinkedListNode5<T>(value);

                Container* nextContainerPointer = &next.container;

                Copy((byte*)thisPointer, (byte*)nextContainerPointer, Unsafe.SizeOf<Container>());

                next.previous = (LinkedListNode5<T>*)nextContainerPointer;

                return next;
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

                if (this.node.previous == null)
                {
                    return false;
                }


                this.node = *node.previous;
                return true;
            }
        }
    }
}
