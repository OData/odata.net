namespace V2.Fx.Collections
{
    using System;
    using System.Collections;
    using V2.Fx.Runtime.CompilerServices;

    //// TODO double check that this doesn't have any `unsafe` contexts

    public ref struct LinkedList<T> where T : allows ref struct
    {
        private BetterReadOnlySpan<LinkedListNode> first;

        private BetterReadOnlySpan<LinkedListNode> current;

        private bool hasValues;

        public LinkedList()
        {
            this.hasValues = false;
        }

        public LinkedList(T value, DifferentMemory memory) //// TODO can you use betterspan instead of span? how about readonlyspan?
        {
            //// TODO do you still want this constructor now that empty lists are a thing?
            this.SetFirstValue(value, memory);
        }

        private void SetFirstValue(T value, DifferentMemory memory) //// TODO can you use betterspan instead of span? how about readonlyspan?
        {
            var firstNode = new LinkedListNode(value);
            Unsafe.Copy(memory, firstNode);

            this.first = BetterReadOnlySpan.FromMemory<LinkedListNode>(memory, 1);
            this.current = this.first;

            this.hasValues = true;
        }

        public void Append(T value, DifferentMemory memory)
        {
            if (!this.hasValues)
            {
                this.SetFirstValue(value, memory);
            }
            else
            {
                var nextNode = new LinkedListNode(value);
                Unsafe.Copy(memory, nextNode);

                var next = BetterReadOnlySpan.FromMemory<LinkedListNode>(memory, 1);

                this.current[0].Next = next;

                this.current = next;
            }
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
            if (this.hasValues)
            {
                return new Enumerator(this.first);
            }
            else
            {
                return new Enumerator();
            }
        }

        public ref struct Enumerator
        {
            private BetterReadOnlySpan<LinkedListNode> node;

            private bool hasMoved;

            private readonly bool hasValues;

            internal Enumerator(BetterReadOnlySpan<LinkedListNode> node)
            {
                this.node = node;

                this.hasMoved = false;
                this.hasValues = true;
            }

            public T Current //// TODO cna you make this return `ref t`?
            {
                get
                {
                    if (!this.hasValues || !this.hasMoved)
                    {
                        throw new Exception("TODO");
                    }

                    return this.node[0].Value;
                }
            }

            public bool MoveNext()
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

                var nextNode = this.node[0].Next;
                if (nextNode.Length != 0)
                {
                    this.node = nextNode;
                    return true;
                }

                return false;
            }
        }
    }
}
