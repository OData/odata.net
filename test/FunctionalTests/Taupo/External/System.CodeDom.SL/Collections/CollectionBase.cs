//---------------------------------------------------------------------
// <copyright file="CollectionBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Collections {
    using System;
    using System.Collections.Generic;

    // Useful base class for typed read/write collections where items derive from object
    public abstract class CollectionBase : IList {
        List<object> list;

        protected CollectionBase() {
            list = new List<object>();
        }
        
        protected CollectionBase(int capacity) {
            list = new List<object>(capacity);
        }

        protected IList InnerList
        {
            get
            {
                if (list == null)
                    list = new List<object>();
                return list;
            }
        }

        protected IList List {
            get { return (IList)this; }
        }

        [System.Runtime.InteropServices.ComVisible(false)]        
        public int Capacity {
            get {
                throw new NotImplementedException();
                //return InnerList.Capacity;
            }
            set {
                throw new NotImplementedException();
                //InnerList.Capacity = value;
            }
        }


        public int Count {
            get {
                return list == null ? 0 : list.Count;
            }
        }

        public void Clear() {
            OnClear();
            InnerList.Clear();
            OnClearComplete();
        }

        public void RemoveAt(int index) {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");
            Object temp = InnerList[index];
            OnValidate(temp);
            OnRemove(index, temp);
            InnerList.RemoveAt(index);
            try {
                OnRemoveComplete(index, temp);
            }
            catch {
                InnerList.Insert(index, temp);
                throw;
            }

        }

        bool IList.IsReadOnly {
            get { return InnerList.IsReadOnly; }
        }

        bool IList.IsFixedSize {
            get { return InnerList.IsFixedSize; }
        }

        bool ICollection.IsSynchronized {
            get { return InnerList.IsSynchronized; }
        }

        Object ICollection.SyncRoot {
            get { return InnerList.SyncRoot; }
        }

        void ICollection.CopyTo(Array array, int index) {
            InnerList.CopyTo(array, index);
        }

        Object IList.this[int index] {
            get { 
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index");
                return InnerList[index]; 
            }
            set { 
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index");
                OnValidate(value);
                Object temp = InnerList[index];
                OnSet(index, temp, value); 
                InnerList[index] = value; 
                try {
                    OnSetComplete(index, temp, value);
                }
                catch {
                    InnerList[index] = temp; 
                    throw;
                }
            }
        }

        bool IList.Contains(Object value) {
            return InnerList.Contains(value);
        }

        int IList.Add(Object value) {
            OnValidate(value);
            OnInsert(InnerList.Count, value);
            int index = InnerList.Add(value);
            try {
                OnInsertComplete(index, value);
            }
            catch {
                InnerList.RemoveAt(index);
                throw;
            }
            return index;
        }

       
        void IList.Remove(Object value) {
            OnValidate(value);
            int index = InnerList.IndexOf(value);
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            OnRemove(index, value);
            InnerList.RemoveAt(index);
            try{
                OnRemoveComplete(index, value);
            }
            catch {
                InnerList.Insert(index, value);
                throw;
            }
        }

        int IList.IndexOf(Object value) {
            return InnerList.IndexOf(value);
        }

        void IList.Insert(int index, Object value) {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");
            OnValidate(value);
            OnInsert(index, value);
            InnerList.Insert(index, value);
            try {
                OnInsertComplete(index, value);
            }
            catch {
                InnerList.RemoveAt(index);
                throw;
            }
        }

        public IEnumerator GetEnumerator() {
            return InnerList.GetEnumerator();
        }

        protected virtual void OnSet(int index, Object oldValue, Object newValue) { 
        }

        protected virtual void OnInsert(int index, Object value) { 
        }

        protected virtual void OnClear() { 
        }

        protected virtual void OnRemove(int index, Object value) { 
        }

        protected virtual void OnValidate(Object value) { 
            if (value == null) throw new ArgumentNullException("value");
        }

        protected virtual void OnSetComplete(int index, Object oldValue, Object newValue) { 
        }

        protected virtual void OnInsertComplete(int index, Object value) { 
        }

        protected virtual void OnClearComplete() { 
        }

        protected virtual void OnRemoveComplete(int index, Object value) { 
        }
    
    }

}
