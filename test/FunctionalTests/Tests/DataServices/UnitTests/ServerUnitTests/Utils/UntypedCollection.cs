//---------------------------------------------------------------------
// <copyright file="UntypedCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Implementation of ICollection of T which can return any type of object from its IEnumerable implementation.</summary>
    /// <typeparam name="T">The type of an item. This is just declared, the actual types can be completely different.</typeparam>
    public class UntypedCollection<T> : ICollection<T>, IList, IEnumerable
    {
        List<object> storage;

        public UntypedCollection()
        {
            storage = new List<object>();
        }

        public UntypedCollection(IEnumerable items)
        {
            storage = new List<object>(items.Cast<object>());
        }

        public void Add(T item)
        {
            storage.Add(item);
        }

        void ICollection<T>.Add(T item)
        {
            storage.Add(item);
        }

        void ICollection<T>.Clear()
        {
            storage.Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return storage.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        int ICollection<T>.Count
        {
            get { return storage.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            return storage.Remove(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        int IList.Add(object value)
        {
            return ((IList)storage).Add(value);
        }

        void IList.Clear()
        {
            storage.Clear();
        }

        bool IList.Contains(object value)
        {
            return storage.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool IList.IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        bool IList.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        void IList.Remove(object value)
        {
            storage.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                return storage[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get { return storage.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
    }
}