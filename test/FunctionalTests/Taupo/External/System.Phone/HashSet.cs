//---------------------------------------------------------------------
// <copyright file="HashSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic {

    /// <summary>
    /// Implementation notes:
    /// This uses an array-based implementation similar to Dictionary<T>, using a buckets array
    /// to map hash values to the Slots array. Items in the Slots array that hash to the same value
    /// are chained together through the "next" indices. 
    /// 
    /// The capacity is always prime; so during resizing, the capacity is chosen as the next prime
    /// greater than double the last capacity. 
    /// 
    /// The underlying data structures are lazily initialized. Because of the observation that, 
    /// in practice, hashtables tend to contain only a few elements, the initial capacity is
    /// set very small (3 elements) unless the ctor with a collection is used.
    /// 
    /// The +/- 1 modifications in methods that add, check for containment, etc allow us to 
    /// distinguish a hash code of 0 from an uninitialized bucket. This saves us from having to 
    /// reset each bucket to -1 when resizing. See Contains, for example.
    /// 
    /// Set methods such as UnionWith, IntersectWith, ExceptWith, and SymmetricExceptWith modify
    /// this set.
    /// 
    /// Some operations can perform faster if we can assume "other" contains unique elements
    /// according to this equality comparer. The only times this is efficient to check is if
    /// other is a hashset. Note that checking that it's a hashset alone doesn't suffice; we
    /// also have to check that the hashset is using the same equality comparer. If other 
    /// has a different equality comparer, it will have unique elements according to its own
    /// equality comparer, but not necessarily according to ours. Therefore, to go these 
    /// optimized routes we check that other is a hashset using the same equality comparer.
    /// 
    /// A HashSet with no elements has the properties of the empty set. (See IsSubset, etc. for 
    /// special empty set checks.)
    /// 
    /// A couple of methods have a special case if other is this (e.g. SymmetricExceptWith). 
    /// If we didn't have these checks, we could be iterating over the set and modifying at
    /// the same time. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(System.Collections.Generic.HashSetDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [SuppressMessage("Microsoft.Naming","CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="By design")]
    public class HashSet<T> : ICollection<T> {

        // store lower 31 bits of hash code
        private const int Lower31BitMask = 0x7FFFFFFF;
        // factor used to increase hashset capacity
        private const int GrowthFactor = 2;
        // cutoff point, above which we won't do stackallocs. This corresponds to 100 integers.
        private const int StackAllocThreshold = 100;
        // when constructing a hashset from an existing collection, it may contain duplicates, 
        // so this is used as the max acceptable excess ratio of capacity to count. Note that
        // this is only used on the ctor and not to automatically shrink if the hashset has, e.g,
        // a lot of adds followed by removes. Users must explicitly shrink by calling TrimExcess.
        // This is set to 3 because capacity is acceptable as 2x rounded up to nearest prime.
        private const int ShrinkThreshold = 3;

        // constants for serialization
        private const String CapacityName = "Capacity";
        private const String ElementsName = "Elements";
        private const String ComparerName = "Comparer";
        private const String VersionName = "Version";

        private int[] m_buckets;
        private Slot[] m_slots;
        private int m_count;
        private int m_lastIndex;
        private int m_freeList;
        private IEqualityComparer<T> m_comparer;
        private int m_version;

        #region Constructors

        public HashSet()
            : this(EqualityComparer<T>.Default) { }

        public HashSet(IEqualityComparer<T> comparer) {
            if (comparer == null) {
                comparer = EqualityComparer<T>.Default;
            }

            this.m_comparer = comparer;
            m_lastIndex = 0;
            m_count = 0;
            m_freeList = -1;
            m_version = 0;
        }

        public HashSet(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default) { }

        /// <summary>
        /// Implementation Notes:
        /// Since resizes are relatively expensive (require rehashing), this attempts to minimize 
        /// the need to resize by setting the initial capacity based on size of collection. 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="comparer"></param>
        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : this(comparer) {
            if (collection == null) {
                throw new ArgumentNullException("collection");
            }

            // to avoid excess resizes, first set size based on collection's count. Collection
            // may contain duplicates, so call TrimExcess if resulting hashset is larger than
            // threshold
            int suggestedCapacity = 0;
            ICollection<T> coll = collection as ICollection<T>;
            if (coll != null) {
                suggestedCapacity = coll.Count;
            }
            Initialize(suggestedCapacity);

            this.UnionWith(collection);
            if ((m_count == 0 && m_slots.Length > HashHelpers.GetMinPrime()) ||
                (m_count > 0 && m_slots.Length / m_count > ShrinkThreshold)) {
                TrimExcess();
            }
        }

        #endregion

        #region ICollection<T> methods

        /// <summary>
        /// Add item to this hashset. This is the explicit implementation of the ICollection<T>
        /// interface. The other Add method returns bool indicating whether item was added.
        /// </summary>
        /// <param name="item">item to add</param>
        void ICollection<T>.Add(T item) {
            AddIfNotPresent(item);
        }

        /// <summary>
        /// Remove all items from this set. This clears the elements but not the underlying 
        /// buckets and slots array. Follow this call by TrimExcess to release these.
        /// </summary>
        public void Clear() {
            if (m_lastIndex > 0) {
                Debug.Assert(m_buckets != null, "m_buckets was null but m_lastIndex > 0");

                // clear the elements so that the gc can reclaim the references.
                // clear only up to m_lastIndex for m_slots 
                Array.Clear(m_slots, 0, m_lastIndex);
                Array.Clear(m_buckets, 0, m_buckets.Length);
                m_lastIndex = 0;
                m_count = 0;
                m_freeList = -1;
            }
            m_version++;
        }

        /// <summary>
        /// Checks if this hashset contains the item
        /// </summary>
        /// <param name="item">item to check for containment</param>
        /// <returns>true if item contained; false if not</returns>
        public bool Contains(T item) {
            if (m_buckets != null) {
                int hashCode = InternalGetHashCode(item);
                // see note at "HashSet" level describing why "- 1" appears in for loop
                for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next) {
                    if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, item)) {
                        return true;
                    }
                }
            }
            // either m_buckets is null or wasn't found
            return false;
        }

        /// <summary>
        /// Copy items in this hashset to array, starting at arrayIndex
        /// </summary>
        /// <param name="array">array to add items to</param>
        /// <param name="arrayIndex">index to start at</param>
        public void CopyTo(T[] array, int arrayIndex) {
            CopyTo(array, arrayIndex, m_count);
        }

        /// <summary>
        /// Remove item from this hashset
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>true if removed; false if not (i.e. if the item wasn't in the HashSet)</returns>
        public bool Remove(T item) {
            if (m_buckets != null) {
                int hashCode = InternalGetHashCode(item);
                int bucket = hashCode % m_buckets.Length;
                int last = -1;
                for (int i = m_buckets[bucket] - 1; i >= 0; last = i, i = m_slots[i].next) {
                    if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, item)) {
                        if (last < 0) {
                            // first iteration; update buckets
                            m_buckets[bucket] = m_slots[i].next + 1;
                        }
                        else {
                            // subsequent iterations; update 'next' pointers
                            m_slots[last].next = m_slots[i].next;
                        }
                        m_slots[i].hashCode = -1;
                        m_slots[i].value = default(T);
                        m_slots[i].next = m_freeList;

                        m_count--;
                        m_version++;
                        if (m_count == 0) {
                            m_lastIndex = 0;
                            m_freeList = -1;
                        }
                        else {
                            m_freeList = i;
                        }
                        return true;
                    }
                }
            }
            // either m_buckets is null or wasn't found
            return false;
        }

        /// <summary>
        /// Number of elements in this hashset
        /// </summary>
        public int Count {
            get { return m_count; }
        }

        /// <summary>
        /// Whether this is readonly
        /// </summary>
        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }

        #endregion

        #region IEnumerable methods

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(this);
        }

        #endregion

        #region HashSet methods

        /// <summary>
        /// Add item to this HashSet. Returns bool indicating whether item was added (won't be 
        /// added if already present)
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if added, false if already present</returns>
        public bool Add(T item) {
            return AddIfNotPresent(item);
        }

        /// <summary>
        /// Take the union of this HashSet with other. Modifies this set.
        /// 
        /// Implementation note: GetSuggestedCapacity (to increase capacity in advance avoiding 
        /// multiple resizes ended up not being useful in practice; quickly gets to the 
        /// point where it's a wasteful check.
        /// </summary>
        /// <param name="other">enumerable with items to add</param>
        public void UnionWith(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException("other");
            }

            foreach (T item in other) {
                AddIfNotPresent(item);
            }
        }

        /// <summary>
        /// Remove items in other from this set. Modifies this set.
        /// </summary>
        /// <param name="other">enumerable with items to remove</param>
        public void ExceptWith(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException("other");
            }
            // this is already the enpty set; return
            if (m_count == 0) {
                return;
            }

            // special case if other is this; a set minus itself is the empty set
            if (other == this) {
                Clear();
                return;
            }

            // remove every element in other from this
            foreach (T element in other) {
                Remove(element);
            }
        }

        /// <summary>
        /// Checks if this is a superset of other
        /// 
        /// Implementation Notes:
        /// The following properties are used up-front to avoid element-wise checks:
        /// 1. If other has no elements (it's the empty set), then this is a superset, even if this
        /// is also the empty set.
        /// 2. If other has unique elements according to this equality comparer, and this has less 
        /// than the number of elements in other, then this can't be a superset
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this is a superset of other; false if not</returns>
        public bool IsSupersetOf(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException("other");
            }

            // try to fall out early based on counts
            ICollection<T> otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null) {
                // if other is the empty set then this is a superset
                if (otherAsCollection.Count == 0) {
                    return true;
                }
                HashSet<T> otherAsSet = other as HashSet<T>;
                // try to compare based on counts alone if other is a hashset with
                // same equality comparer
                if (otherAsSet != null && AreEqualityComparersEqual(this, otherAsSet)) {
                    if (otherAsSet.Count > m_count) {
                        return false;
                    }
                }
            }

            return ContainsAllElements(other);
        }

        /// <summary>
        /// Checks if this set overlaps other (i.e. they share at least one item)
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if these have at least one common element; false if disjoint</returns>
        public bool Overlaps(IEnumerable<T> other) {
            if (other == null) {
                throw new ArgumentNullException("other");
            }
            if (m_count == 0) {
                return false;
            }

            foreach (T element in other) {
                if (Contains(element)) {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array) { CopyTo(array, 0, m_count); }

        public void CopyTo(T[] array, int arrayIndex, int count) {
            if (array == null) {
                throw new ArgumentNullException("array");
            }

            // check array index valid index into array
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException("arrayIndex", SR.GetString(SR.ArgumentOutOfRange_NeedNonNegNum));
            }

            // also throw if count less than 0
            if (count < 0) {
                throw new ArgumentOutOfRangeException("count", SR.GetString(SR.ArgumentOutOfRange_NeedNonNegNum));
            }

            // will array, starting at arrayIndex, be able to hold elements? Note: not
            // checking arrayIndex >= array.Length (consistency with list of allowing
            // count of 0; subsequent check takes care of the rest)
            if (arrayIndex > array.Length || count > array.Length - arrayIndex) {
                throw new ArgumentException(SR.GetString(SR.Arg_ArrayPlusOffTooSmall));
            }

            int numCopied = 0;
            for (int i = 0; i < m_lastIndex && numCopied < count; i++) {
                if (m_slots[i].hashCode >= 0) {
                    array[arrayIndex + numCopied] = m_slots[i].value;
                    numCopied++;
                }
            }
        }

        /// <summary>
        /// Remove elements that match specified predicate. Returns the number of elements removed
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int RemoveWhere(Predicate<T> match) {
            if (match == null) {
                throw new ArgumentNullException("match");
            }

            int numRemoved = 0;
            for (int i = 0; i < m_lastIndex; i++) {
                if (m_slots[i].hashCode >= 0) {
                    // cache value in case delegate removes it
                    T value = m_slots[i].value;
                    if (match(value)) {
                        // check again that remove actually removed it
                        if (Remove(value)) {
                            numRemoved++;
                        }
                    }
                }
            }
            return numRemoved;
        }

        /// <summary>
        /// Gets the IEqualityComparer that is used to determine equality of keys for 
        /// the HashSet.
        /// </summary>
        public IEqualityComparer<T> Comparer {
            get {
                return m_comparer;
            }
        }

        /// <summary>
        /// Sets the capacity of this list to the size of the list (rounded up to nearest prime),
        /// unless count is 0, in which case we release references.
        /// 
        /// This method can be used to minimize a list's memory overhead once it is known that no
        /// new elements will be added to the list. To completely clear a list and release all 
        /// memory referenced by the list, execute the following statements:
        /// 
        /// list.Clear();
        /// list.TrimExcess(); 
        /// </summary>
        public void TrimExcess() {
            Debug.Assert(m_count >= 0, "m_count is negative");

            if (m_count == 0) {
                // if count is zero, clear references
                m_buckets = null;
                m_slots = null;
                m_version++;
            }
            else {
                Debug.Assert(m_buckets != null, "m_buckets was null but m_count > 0");

                // similar to IncreaseCapacity but moves down elements in case add/remove/etc
                // caused fragmentation
                int newSize = HashHelpers.GetPrime(m_count);
                Slot[] newSlots = new Slot[newSize];
                int[] newBuckets = new int[newSize];

                // move down slots and rehash at the same time. newIndex keeps track of current 
                // position in newSlots array
                int newIndex = 0;
                for (int i = 0; i < m_lastIndex; i++) {
                    if (m_slots[i].hashCode >= 0) {
                        newSlots[newIndex] = m_slots[i];

                        // rehash
                        int bucket = newSlots[newIndex].hashCode % newSize;
                        newSlots[newIndex].next = newBuckets[bucket] - 1;
                        newBuckets[bucket] = newIndex + 1;

                        newIndex++;
                    }
                }

                Debug.Assert(newSlots.Length <= m_slots.Length, "capacity increased after TrimExcess");

                m_lastIndex = newIndex;
                m_slots = newSlots;
                m_buckets = newBuckets;
                m_freeList = -1;
            }
        }

        /// <summary>
        /// Used for deep equality of HashSet testing
        /// </summary>
        /// <returns></returns>
        public static IEqualityComparer<HashSet<T>> CreateSetComparer() {
            return new HashSetEqualityComparer<T>();
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Initializes buckets and slots arrays. Uses suggested capacity by finding next prime
        /// greater than or equal to capacity.
        /// </summary>
        /// <param name="capacity"></param>
        private void Initialize(int capacity) {
            Debug.Assert(m_buckets == null, "Initialize was called but m_buckets was non-null");

            int size = HashHelpers.GetPrime(capacity);

            m_buckets = new int[size];
            m_slots = new Slot[size];
        }

        /// <summary>
        /// Expand to new capacity. New capacity is next prime greater than or equal to suggested 
        /// size. This is called when the underlying array is filled. This performs no 
        /// defragmentation, allowing faster execution; note that this is reasonable since 
        /// AddIfNotPresent attempts to insert new elements in re-opened spots.
        /// </summary>
        /// <param name="sizeSuggestion"></param>
        private void IncreaseCapacity() {
            Debug.Assert(m_buckets != null, "IncreaseCapacity called on a set with no elements");

            // Handle overflow conditions. Try to expand capacity by GrowthFactor. If that causes
            // overflow, use size suggestion of m_count and see if HashHelpers returns a value
            // greater than that. If not, capacity can't be increased so throw capacity overflow 
            // exception.
            int sizeSuggestion = unchecked(m_count * GrowthFactor);
            if (sizeSuggestion < 0) {
                sizeSuggestion = m_count;
            }
            int newSize = HashHelpers.GetPrime(sizeSuggestion);
            if (newSize <= m_count) {
                throw new ArgumentException(SR.GetString(SR.Arg_HSCapacityOverflow));
            }

            // Able to increase capacity; copy elements to larger array and rehash
            Slot[] newSlots = new Slot[newSize];
            if (m_slots != null) {
                Array.Copy(m_slots, 0, newSlots, 0, m_lastIndex);
            }

            int[] newBuckets = new int[newSize];
            for (int i = 0; i < m_lastIndex; i++) {
                int bucket = newSlots[i].hashCode % newSize;
                newSlots[i].next = newBuckets[bucket] - 1;
                newBuckets[bucket] = i + 1;
            }
            m_slots = newSlots;
            m_buckets = newBuckets;

        }

        /// <summary>
        /// Adds value to HashSet if not contained already
        /// Returns true if added and false if already present
        /// </summary>
        /// <param name="value">value to find</param>
        /// <returns></returns>
        private bool AddIfNotPresent(T value) {
            if (m_buckets == null) {
                Initialize(0);
            }

            int hashCode = InternalGetHashCode(value);
            int bucket = hashCode % m_buckets.Length;
            for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next) {
                if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, value)) {
                    return false;
                }
            }
            int index;
            if (m_freeList >= 0) {
                index = m_freeList;
                m_freeList = m_slots[index].next;
            }
            else {
                if (m_lastIndex == m_slots.Length) {
                    IncreaseCapacity();
                    // this will change during resize
                    bucket = hashCode % m_buckets.Length;
                }
                index = m_lastIndex;
                m_lastIndex++;
            }
            m_slots[index].hashCode = hashCode;
            m_slots[index].value = value;
            m_slots[index].next = m_buckets[bucket] - 1;
            m_buckets[bucket] = index + 1;
            m_count++;
            m_version++;
            return true;
        }

        /// <summary>
        /// Checks if this contains of other's elements. Iterates over other's elements and 
        /// returns false as soon as it finds an element in other that's not in this.
        /// Used by SupersetOf, ProperSupersetOf, and SetEquals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool ContainsAllElements(IEnumerable<T> other) {
            foreach (T element in other) {
                if (!Contains(element)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Implementation Notes:
        /// If other is a hashset and is using same equality comparer, then checking subset is 
        /// faster. Simply check that each element in this is in other.
        /// 
        /// Note: if other doesn't use same equality comparer, then Contains check is invalid,
        /// which is why callers must take are of this.
        /// 
        /// If callers are concerned about whether this is a proper subset, they take care of that.
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool IsSubsetOfHashSetWithSameEC(HashSet<T> other) {

            foreach (T item in this) {
                if (!other.Contains(item)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// If other is a hashset that uses same equality comparer, intersect is much faster 
        /// because we can use other's Contains
        /// </summary>
        /// <param name="other"></param>
        private void IntersectWithHashSetWithSameEC(HashSet<T> other) {
            for (int i = 0; i < m_lastIndex; i++) {
                if (m_slots[i].hashCode >= 0) {
                    T item = m_slots[i].value;
                    if (!other.Contains(item)) {
                        Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// Used internally by set operations which have to rely on bit array marking. This is like
        /// Contains but returns index in slots array. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int InternalIndexOf(T item) {
            Debug.Assert(m_buckets != null, "m_buckets was null; callers should check first");

            int hashCode = InternalGetHashCode(item);
            for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next) {
                if ((m_slots[i].hashCode) == hashCode && m_comparer.Equals(m_slots[i].value, item)) {
                    return i;
                }
            }
            // wasn't found
            return -1;
        }

        /// <summary>
        /// if other is a set, we can assume it doesn't have duplicate elements, so use this
        /// technique: if can't remove, then it wasn't present in this set, so add.
        /// 
        /// As with other methods, callers take care of ensuring that other is a hashset using the
        /// same equality comparer.
        /// </summary>
        /// <param name="other"></param>
        private void SymmetricExceptWithUniqueHashSet(HashSet<T> other) {
            foreach (T item in other) {
                if (!Remove(item)) {
                    AddIfNotPresent(item);
                }
            }
        }

        /// <summary>
        /// Add if not already in hashset. Returns an out param indicating index where added. This 
        /// is used by SymmetricExcept because it needs to know the following things:
        /// - whether the item was already present in the collection or added from other
        /// - where it's located (if already present, it will get marked for removal, otherwise
        /// marked for keeping)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private bool AddOrGetLocation(T value, out int location) {
            Debug.Assert(m_buckets != null, "m_buckets is null, callers should have checked");

            int hashCode = InternalGetHashCode(value);
            int bucket = hashCode % m_buckets.Length;
            for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next) {
                if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, value)) {
                    location = i;
                    return false; //already present
                }
            }
            int index;
            if (m_freeList >= 0) {
                index = m_freeList;
                m_freeList = m_slots[index].next;
            }
            else {
                if (m_lastIndex == m_slots.Length) {
                    IncreaseCapacity();
                    // this will change during resize
                    bucket = hashCode % m_buckets.Length;
                }
                index = m_lastIndex;
                m_lastIndex++;
            }
            m_slots[index].hashCode = hashCode;
            m_slots[index].value = value;
            m_slots[index].next = m_buckets[bucket] - 1;
            m_buckets[bucket] = index + 1;
            m_count++;
            m_version++;
            location = index;
            return true;
        }

        /// <summary>
        /// Copies this to an array. Used for DebugView
        /// </summary>
        /// <returns></returns>
        internal T[] ToArray() {
            T[] newArray = new T[Count];
            CopyTo(newArray);
            return newArray;
        }

        /// <summary>
        /// Internal method used for HashSetEqualityComparer. Compares set1 and set2 according 
        /// to specified comparer.
        /// 
        /// Because items are hashed according to a specific equality comparer, we have to resort
        /// to n^2 search if they're using different equality comparers.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        internal static bool HashSetEquals(HashSet<T> set1, HashSet<T> set2, IEqualityComparer<T> comparer) {
            // handle null cases first
            if (set1 == null) {
                return (set2 == null);
            }
            else if (set2 == null) {
                // set1 != null
                return false;
            }

            // all comparers are the same; this is faster
            if (AreEqualityComparersEqual(set1, set2)) {
                if (set1.Count != set2.Count) {
                    return false;
                }
                // suffices to check subset
                foreach (T item in set2) {
                    if (!set1.Contains(item)) {
                        return false;
                    }
                }
                return true;
            }
            else {  // n^2 search because items are hashed according to their respective ECs
                foreach (T set2Item in set2) {
                    bool found = false;
                    foreach (T set1Item in set1) {
                        if (comparer.Equals(set2Item, set1Item)) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using 
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        private static bool AreEqualityComparersEqual(HashSet<T> set1, HashSet<T> set2) {
            return set1.Comparer.Equals(set2.Comparer);
        }

        /// <summary>
        /// Workaround Comparers that throw ArgumentNullException for GetHashCode(null).
        /// </summary>
        /// <param name="item"></param>
        /// <returns>hash code</returns>
        private int InternalGetHashCode(T item) {
            if (item == null) {
                return 0;
            } 
            return m_comparer.GetHashCode(item) & Lower31BitMask;
        }

        #endregion

        internal struct Slot {
            internal int hashCode;      // Lower 31 bits of hash code, -1 if unused
            internal T value;
            internal int next;          // Index of next entry, -1 if last
        }

        public struct Enumerator : IEnumerator<T>, System.Collections.IEnumerator {
            private HashSet<T> set;
            private int index;
            private int version;
            private T current;

            internal Enumerator(HashSet<T> set) {
                this.set = set;
                index = 0;
                version = set.m_version;
                current = default(T);
            }

            public void Dispose() {
            }

            public bool MoveNext() {
                if (version != set.m_version) {
                    throw new InvalidOperationException(SR.GetString(SR.InvalidOperation_EnumFailedVersion));
                }

                while (index < set.m_lastIndex) {
                    if (set.m_slots[index].hashCode >= 0) {
                        current = set.m_slots[index].value;
                        index++;
                        return true;
                    }
                    index++;
                }
                index = set.m_lastIndex + 1;
                current = default(T);
                return false;
            }

            public T Current {
                get {
                    return current;
                }
            }

            Object System.Collections.IEnumerator.Current {
                get {
                    if (index == 0 || index == set.m_lastIndex + 1) {
                        throw new InvalidOperationException(SR.GetString(SR.InvalidOperation_EnumOpCantHappen));
                    }
                    return Current;
                }
            }

            void System.Collections.IEnumerator.Reset() {
                if (version != set.m_version) {
                    throw new InvalidOperationException(SR.GetString(SR.InvalidOperation_EnumFailedVersion));
                }

                index = 0;
                current = default(T);
            }
        }
    }

}
