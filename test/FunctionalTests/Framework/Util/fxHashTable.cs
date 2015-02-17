//---------------------------------------------------------------------
// <copyright file="fxHashTable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;                   //IEnumerable
using System.Collections.Generic;           //Dictionary<K, V>

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// AstoriaList
	//
    //  Note: We have to 'contain' Dictionary<K, V>, since methods on Distionary are not overridable, argh!
	////////////////////////////////////////////////////////   
    public class AstoriaHashTable<K, V> : IDictionary<K, V>
    {
        //Data
        protected Dictionary<K, V> _hash;
        
        //Constructor
        public AstoriaHashTable()
        {
            _hash = new Dictionary<K, V>();
        }

        //IDictionary methods
        public virtual V                    this[K key] 
        { 
            get { return _hash[key];            }
            set { _hash[key] = value;           }
        }

        public virtual ICollection<K>       Keys 
        {
            get { return _hash.Keys;            }        
        }
        
        public virtual ICollection<V>       Values 
        { 
            get { return _hash.Values;          }
        }

        public virtual void                 Add(K key, V value)
        {
            _hash.Add(key, value);
        }

        public virtual bool                 ContainsKey(K key)
        {
            return _hash.ContainsKey(key);
        }
        
        public virtual bool                 Remove(K key)
        {
            return _hash.Remove(key);
        }
        
        public virtual bool                 TryGetValue(K key, out V value)
        {
            return _hash.TryGetValue(key, out value);
        }

        //ICollection<KeyValuePair<K, V>>
        protected virtual ICollection<KeyValuePair<K, V>> Collection
        {
            get { return _hash;                 }
        }
        
        public virtual int                  Count 
        { 
            get { return this.Collection.Count; }
        }

        public virtual bool                 IsReadOnly 
        { 
            get { return this.Collection.IsReadOnly;}
        }
        
        public virtual void                 Add(KeyValuePair<K, V> item)
        {
            this.Collection.Add(item);
        }
        
        public virtual void                 Clear()
        {
            this.Collection.Clear();
        }
        
        public virtual bool                 Contains(KeyValuePair<K, V> item)
        {
            return this.Collection.Contains(item);
        }
        
        public virtual void                 CopyTo(KeyValuePair<K, V>[] array, int index)
        {
            this.Collection.CopyTo(array, index);
        }
        
        public virtual bool                 Remove(KeyValuePair<K, V> item)
        {
            return this.Collection.Remove(item);
        }

        //IEnumerable<KeyValuePair<K, V>>
        public virtual IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _hash.GetEnumerator();
        }

        //IEnumerable
        IEnumerator                         IEnumerable.GetEnumerator()
        {
            return _hash.GetEnumerator();
        }
    }
}
