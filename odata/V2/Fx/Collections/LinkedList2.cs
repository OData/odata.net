namespace V2.Fx.Collections
{
    using System;
    using System.Runtime.InteropServices;

    public readonly ref struct Pointer<T> where T : allows ref struct
    {
        private readonly FakeSpan<T> span;

        public unsafe Pointer(ref T value)
        {
            fixed (T* pointer = &value)
            {
                this.span = new FakeSpan<T>()
                {
                    Reference = pointer,
                    Length = 1,
                };
            }
        }

        public void Set(ref T value)
        {
            this.span[0] = value;
        }

        public ref T Get()
        {
            return ref this.span[0];
        }

        public bool IsNull()
        {
            return this.span.Length == 0;
        }
    }

    public unsafe ref struct FakeSpan<T> where T : allows ref struct
    {
        public T* Reference;

        public int Length;

        public ref T this[int index]
        {
            get
            {
                return ref *Reference;
            }
        }
    }

    public ref struct LinkedList2<T> where T : allows ref struct
    {
        private Pointer<Node> first;

        private Pointer<Node> current;

        private bool hasValues;

        public LinkedList2(Span<byte> emptySpan)
        {
            if (emptySpan.Length != 0)
            {
                // this is to protect the caller from overallocating memory; we don't use this span for anything, it only exists
                // for compiler checks on memory integrity
                var message = $"A new list must be instantiated with empty data. The provided data has length '{emptySpan.Length}'";
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            this.hasValues = false;
        }

        public unsafe void Append(T value, Span<byte> memory)
        {
            var nextNode = new Node()
            {
                Element = value,
                Next = new Pointer<Node>(),
            };

            V2.Fx.Runtime.InteropServices.MemoryMarshal.Write(memory, nextNode);
            var intermediate = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<Node>(ref System.Runtime.InteropServices.MemoryMarshal.GetReference(memory));

            var pointer = new Pointer<Node>(ref intermediate);
            if (!this.hasValues)
            {
                this.first = pointer;
                this.current = this.first;
            }
            else
            {
                this.current.Get().Next = pointer;
            }
        }

        public int MemorySize { get; } = System.Runtime.CompilerServices.Unsafe.SizeOf<Node>();

        private ref struct Node
        {
            public T Element;

            public Pointer<Node> Next;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private bool hasValues;

            private Pointer<Node> current;

            private bool hasMoved;

            public Enumerator(LinkedList2<T> list)
            {
                this.hasValues = list.hasValues;
                this.current = list.first;

                this.hasValues = false;
            }

            public unsafe T Current
            {
                get
                {
                    if (!this.hasMoved)
                    {
                        throw new Exception("TODO");
                    }

                    return this.current.Get().Element;
                }
            }

            public unsafe bool MoveNext()
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

                var next = this.current.Get().Next;
                if (next.IsNull())
                {
                    return false;
                }

                this.current = next;
                return true;
            }
        }
    }
}
