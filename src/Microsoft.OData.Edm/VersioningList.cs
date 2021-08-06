//---------------------------------------------------------------------
// <copyright file="VersioningList.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Provides a list that is thread safe by virtue of being immutable.
    /// "Mutating" operations return a new list (which, for efficiency, may share some of the state of the old one).
    /// </summary>
    /// <typeparam name="TElement">Element type of the list.</typeparam>
    internal abstract class VersioningList<TElement> : IReadOnlyList<TElement>, IEnumerable<TElement>
    {
        public abstract int Count { get; }

        public TElement this[int index]
        {
            get
            {
                if (((uint)index) >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return this.IndexedElement(index);
            }
        }

        public static VersioningList<TElement> Create()
        {
            return new EmptyVersioningList();
        }

        public abstract VersioningList<TElement> Add(TElement value);

        public VersioningList<TElement> RemoveAt(int index)
        {
            if (((uint)index) >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return this.RemoveIndexedElement(index);
        }

        public abstract IEnumerator<TElement> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected abstract TElement IndexedElement(int index);

        protected abstract VersioningList<TElement> RemoveIndexedElement(int index);

        internal sealed class EmptyVersioningList : VersioningList<TElement>
        {
            public override int Count
            {
                get { return 0; }
            }

            public override VersioningList<TElement> Add(TElement value)
            {
                return new ArrayVersioningList(this, value);
            }

            public override IEnumerator<TElement> GetEnumerator()
            {
                return new EmptyListEnumerator();
            }

            protected override TElement IndexedElement(int index)
            {
                throw new IndexOutOfRangeException();
            }

            protected override VersioningList<TElement> RemoveIndexedElement(int index)
            {
                throw new IndexOutOfRangeException();
            }
        }

        internal sealed class EmptyListEnumerator : IEnumerator<TElement>
        {
            public TElement Current
            {
                get { return default(TElement); }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }
        }

        internal sealed class ArrayVersioningList : VersioningList<TElement>
        {
            private readonly TElement[] elements;

            public ArrayVersioningList(VersioningList<TElement> preceding, TElement last)
            {
                this.elements = new TElement[preceding.Count + 1];
                for (int i = 0; i < preceding.Count; i++)
                {
                    this.elements[i] = preceding[i];
                }

                this.elements[preceding.Count] = last;
            }

            private ArrayVersioningList(TElement[] elements)
            {
                this.elements = elements;
            }

            public override int Count
            {
                get { return this.elements.Length; }
            }

            public TElement ElementAt(int index)
            {
                return this.elements[index];
            }

            public override VersioningList<TElement> Add(TElement value)
            {
                return new ArrayVersioningList(this, value);
            }

            public override IEnumerator<TElement> GetEnumerator()
            {
                return new ArrayListEnumerator(this);
            }

            protected override TElement IndexedElement(int index)
            {
                return this.elements[index];
            }

            protected override VersioningList<TElement> RemoveIndexedElement(int index)
            {
                if (this.elements.Length == 1)
                {
                    return new EmptyVersioningList();
                }

                int newIndex = 0;
                TElement[] newElements = new TElement[this.elements.Length - 1];
                for (int oldIndex = 0; oldIndex < this.elements.Length; oldIndex++)
                {
                    if (oldIndex != index)
                    {
                        newElements[newIndex++] = this.elements[oldIndex];
                    }
                }

                return new ArrayVersioningList(newElements);
            }
        }

        internal sealed class ArrayListEnumerator : IEnumerator<TElement>
        {
            private readonly ArrayVersioningList array;
            private int index;

            public ArrayListEnumerator(ArrayVersioningList array)
            {
                this.array = array;
            }

            public TElement Current
            {
                get
                {
                    if (this.index <= this.array.Count)
                    {
                        return this.array.ElementAt(this.index - 1);
                    }

                    return default(TElement);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                int count = this.array.Count;
                if (this.index <= count)
                {
                    this.index++;
                }

                return this.index <= count;
            }

            public void Reset()
            {
                this.index = 0;
            }
        }
    }
}
