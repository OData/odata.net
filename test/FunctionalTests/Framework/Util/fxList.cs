//---------------------------------------------------------------------
// <copyright file="fxList.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;                   //IEnumerable
using System.Collections.Generic;           //List<T>


namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// esList
	//
    //  Note: We have to 'contain' List<T>, since methods are List are not overridable, argh!
	////////////////////////////////////////////////////////   
    public class fxList<T> : IList<T>, IEnumerable<T>
    {
        //Data
        protected List<T> _list;
        
        //Constructor
        public fxList(params T[] array)
        {
            _list = new List<T>(array);
        }

        public fxList(IEnumerable<T> array)
        {
            _list = new List<T>(array);
        }

        //Properties
        public virtual T First
        {
            get { return this.ElementAtOrDefault(0); }
        }

        public virtual T ElementAtOrDefault(int index)
        {
            if (index < _list.Count && index >= 0)
                return _list[index];
            return default(T);
        }

        //IList methods
        public  virtual T           this[int index]
        {
            get { return _list[index];          }
            set { _list[index] = value;         }
        }
        
        public  virtual int         IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public  virtual bool        Contains(T item)
        {
            return _list.Contains(item);
        }

        public  virtual int         Count
        {
            get { return _list.Count;           }
        }

        public  virtual bool       IsReadOnly
        {
            get { return ((ICollection<T>)_list).IsReadOnly;        }
        }

        void                        ICollection<T>.Add(T item)
        {
            //Delegate
            T t = this.Add(item);
        }
        public virtual T[] ToArray()
        {
            return _list.ToArray();
        }
        public  virtual T           Add(T item)
        {
            _list.Add(item);
            return item;
        }

        public  virtual void        Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public  virtual bool        Remove(T item)
        {
            return _list.Remove(item);
        }

        public  virtual void        Remove(IEnumerable<T> array)
        {
            foreach(T item in array)
                this.Remove(item);
        }

        public  virtual void        RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public virtual void		    CopyTo(T[] array, int index)
	    {
            _list.CopyTo(array, index);
	    }

        public  virtual void        Add(params T[] array)
        {
            //Delegate
            this.Add((IEnumerable<T>)array);
        }

        public  virtual void        Add(IEnumerable<T> array)
        {
            foreach(T item in array)
                this.Add(item);
        }

        public virtual T		    Update(T item)
	    {
            if(IndexOf(item) < 0)
                this.Add(item);
            return item;
	    }

        public  virtual void        Clear()
        {
            _list.Clear();
        }

        public virtual IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public virtual T            Choose()
	    {
            if(_list.Count > 0)
		        return _list[System.Data.Test.Astoria.AstoriaTestProperties.Random.Next(_list.Count)];
            return default(T);
	    }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {

            return _list.GetEnumerator();
        }

        #endregion
    }
}
