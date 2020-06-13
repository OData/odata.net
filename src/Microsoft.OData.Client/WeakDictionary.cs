//---------------------------------------------------------------------
// <copyright file="WeakDictionary.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A generic dictionary, which allows its keys to be garbage collected if there are no other references
    /// to them than from the dictionary itself.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <remarks>
    /// If the key of a particular entry in the dictionary has been collected, then the key becomes
    /// effectively unreachable. However, left-over WeakReference objects for the key will physically remain in
    /// the dictionary until RemoveCollectedEntries is called.
    /// </remarks>
    internal class WeakDictionary<TKey, TValue>
        where TKey : class
    {
        /// <summary>
        /// Internal Dictionary which actually stores the key and value.
        /// </summary>
        private Dictionary<object, TValue> dictionary;

        /// <summary>
        /// The comparer used to determine equality of keys for the internal dictionary.
        /// </summary>
        private WeakKeyComparer<TKey> comparer;

        /// <summary>
        /// The interval for calling RemoveCollectedEntries to refresh the internal dictionary.
        /// </summary>
        private int intervalForRefresh;

        /// <summary>
        /// The count limit for calling RemoveCollectedEntries to refresh the internal dictionary.
        /// </summary>
        private int countLimitForRefresh;

        /// <summary>
        /// Count on how many entries has been added.
        /// </summary>
        private int countForRefresh = 0;

        /// <summary>
        /// The constructor with a specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The IEqualityComparer used to determine equals of object of <typeparamref name="TKey"/>.
        /// </param>
        public WeakDictionary(IEqualityComparer<TKey> comparer)
            : this(comparer, 1000)
        {
        }

        /// <summary>
        /// The constructor with a specified comparer and a refresh interval.
        /// </summary>
        /// <param name="comparer">
        /// The IEqualityComparer used to determine equals of object of <typeparamref name="TKey"/>.
        /// </param>
        /// <param name="refreshInterval">
        /// The interval for calling RemoveCollectedEntries to refresh the dictionary.
        /// </param>
        public WeakDictionary(IEqualityComparer<TKey> comparer, int refreshInterval)
        {
            this.comparer = comparer as WeakKeyComparer<TKey>;
            if (this.comparer == null)
            {
                this.comparer = new WeakKeyComparer<TKey>(comparer);
            }

            this.dictionary = new Dictionary<object, TValue>(this.comparer);
            this.intervalForRefresh = refreshInterval;
            this.countLimitForRefresh = refreshInterval;
        }

        /// <summary>
        /// Defines how the keys are stored in this dictionary.
        /// </summary>
        public Func<object, object> CreateWeakKey { get; set; }

        /// <summary>
        /// Get the list of rules that define whether an entry can be removed.
        /// </summary>
        public List<Func<object, bool>> RemoveCollectedEntriesRules { get; set; }

        /// <summary>
        /// Get the count of entries in this dictionary.
        /// </summary>
        /// <remarks>
        /// The count returned here may include entries for which the key has already been garbage collected.
        /// Call RemoveCollectedEntries to weed out collected entries and update the count accordingly.
        /// </remarks>
        public int Count
        {
            get { return this.dictionary.Count; }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key"> The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public TValue this[TKey key]
        {
            get
            {
                return this.dictionary[key];
            }

            set
            {
                this.dictionary[key] = value;
            }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (countForRefresh >= countLimitForRefresh)
            {
                RemoveCollectedEntries();
            }

            if (this.CreateWeakKey != null)
            {
                // Customize the key.
                this.dictionary.Add(CreateWeakKey(key), value);
            }
            else
            {
                this.dictionary.Add(new WeakKeyReference<TKey>(key, this.comparer), value);
            }

            countForRefresh++;
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully found and removed;
        /// otherwise, false if key is not found in the dictionary.</returns>
        public bool Remove(TKey key)
        {
            var removed = this.dictionary.Remove(key);
            if (removed)
            {
                countForRefresh--;
            }

            return removed;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified
        /// key, if the key is found; otherwise, the default value for the type of the value parameter.
        /// </param>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes the left-over weak references for entries in the dictionary whose key has already been
        /// reclaimed by the garbage collector. This will reduce the dictionary's Count by the number of
        /// dead key-value pairs that were eliminated.
        /// </summary>
        public void RemoveCollectedEntries()
        {
            List<object> entriesToBeRemoved = new List<object>();
            foreach (var key in this.dictionary.Keys)
            {
                WeakKeyReference<TKey> weakKey = key as WeakKeyReference<TKey>;
                if (weakKey != null)
                {
                    if (!weakKey.IsAlive)
                    {
                        entriesToBeRemoved.Add(key);
                    }
                }
                else if (this.RemoveCollectedEntriesRules.Any(f => f(key)))
                {
                    entriesToBeRemoved.Add(key);
                }
            }

            foreach (var key in entriesToBeRemoved)
            {
                if (this.dictionary.Remove(key))
                {
                    this.countForRefresh--;
                }
            }

            // After a clean, we will let the dict to clean itself after adding the next intervalForRefresh(1000) items.
            this.countLimitForRefresh = this.countForRefresh + intervalForRefresh;
        }
    }

    /// <summary>
    /// A weak reference for an object of <typeparamref name="T"/>, which can be used as key in a dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the keys in the dictionary.</typeparam>
    internal sealed class WeakKeyReference<T> : WeakReference
        where T : class
    {
        public WeakKeyReference(T target, WeakKeyComparer<T> comparer)
            : base(target, false)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // retain the object's hash code immediately so that even
            // if the target is GC'ed we will be able to find and
            // remove the dead weak reference.
            this.HashCode = comparer.GetHashCode(target);
        }

        /// <summary>
        /// The hash code of the target.
        /// </summary>
        public int HashCode { get; private set; }

        /// <summary>
        ///  Gets the object (the target) referenced by the current weak reference object.
        /// </summary>
        public new T Target
        {
            get { return (T)base.Target; }
        }
    }

    /// <summary>
    /// Defines methods to support the comparison of WeakKeyReferences for equality.
    /// </summary>
    /// <typeparam name="T">The type of the object to be compared.</typeparam>
    /// <remarks>
    /// Compares objects of the given type or WeakKeyReferences to them for equality based on the given comparer. Note that we can only
    /// implement <see cref="IEqualityComparer&lt;T&gt;"/> for T = object as there is no other common base between T and <see cref="WeakKeyReference&lt;T&gt;"/>. We need a
    /// single comparer to handle both types because we don't want to allocate a new weak reference for every lookup.
    /// </remarks>
    internal class WeakKeyComparer<T> : IEqualityComparer<object>
        where T : class
    {
        /// <summary>
        /// Internal comparer used to determine the equal for <typeparamref name="T"/> objects.
        /// </summary>
        protected IEqualityComparer<T> comparer;

        /// <summary>
        /// The default comparer.
        /// </summary>
        private static WeakKeyComparer<T> defaultInstance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparer">
        /// Internal comparer used to determine the equal for <typeparamref name="T"/> objects.
        /// </param>
        public WeakKeyComparer(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            this.comparer = comparer;
        }

        /// <summary>
        /// Get the default comparer.
        /// </summary>
        public static WeakKeyComparer<T> Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new WeakKeyComparer<T>(null);
                }

                return defaultInstance;
            }
        }

        /// <summary>
        /// Get the hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        public virtual int GetHashCode(object obj)
        {
            WeakKeyReference<T> weakKey = obj as WeakKeyReference<T>;
            if (weakKey != null)
            {
                return weakKey.HashCode;
            }

            return this.comparer.GetHashCode((T)obj);
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj1">The first object to compare.</param>
        /// <param name="obj2">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <remarks>
        ///  Note: There are actually 9 cases to handle here.
        ///
        ///  Let Wa = Alive Weak Reference
        ///  Let Wd = Dead Weak Reference
        ///  Let S  = Strong Reference
        ///
        ///  x  | y  | Equals(x,y)
        /// -------------------------------------------------
        ///  Wa | Wa | comparer.Equals(x.Target, y.Target)
        ///  Wa | Wd | false
        ///  Wa | S  | comparer.Equals(x.Target, y)
        ///  Wd | Wa | false
        ///  Wd | Wd | x == y
        ///  Wd | S  | false
        ///  S  | Wa | comparer.Equals(x, y.Target)
        ///  S  | Wd | false
        ///  S  | S  | comparer.Equals(x, y)
        /// -------------------------------------------------
        /// </remarks>
        public virtual new bool Equals(object obj1, object obj2)
        {
            bool obj1IsDead, obj2IsDead;
            T first = GetTarget(obj1, out obj1IsDead);
            T second = GetTarget(obj2, out obj2IsDead);

            if (obj1IsDead)
            {
                return obj2IsDead ? obj1 == obj2 : false;
            }

            if (obj2IsDead)
            {
                return false;
            }

            return this.comparer.Equals(first, second);
        }

        /// <summary>
        /// Gets the target of the input object if it is a <see cref="WeakKeyReference&lt;T&gt;"/>, else return it self.
        /// </summary>
        /// <param name="obj">The input object from which to get the target.</param>
        /// <param name="isDead">Indicate whether the object is dead if it is a <see cref="WeakKeyReference&lt;T&gt;"/>.</param>
        /// <returns>The target of the input object.</returns>
        protected virtual T GetTarget(object obj, out bool isDead)
        {
            T target;

            WeakKeyReference<T> wref = obj as WeakKeyReference<T>;
            if (wref != null)
            {
                target = wref.Target;
                isDead = !wref.IsAlive;
            }
            else
            {
                target = (T)obj;
                isDead = false;
            }

            return target;
        }
    }
}
