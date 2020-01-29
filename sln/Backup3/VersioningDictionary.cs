//---------------------------------------------------------------------
// <copyright file="VersioningDictionary.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Provides a dictionary that is thread safe by virtue of being immutable.
    /// Any update returns a new dictionary (which, for efficiency, may share some of the state of the old one).
    /// </summary>
    /// <typeparam name="TKey">Key type of the dictionary.</typeparam>
    /// <typeparam name="TValue">Value type of the dictionary.</typeparam>
    internal abstract class VersioningDictionary<TKey, TValue>
    {
        protected readonly Func<TKey, TKey, int> CompareFunction;

        protected VersioningDictionary(Func<TKey, TKey, int> compareFunction)
        {
            this.CompareFunction = compareFunction;
        }

        public static VersioningDictionary<TKey, TValue> Create(Func<TKey, TKey, int> compareFunction)
        {
            return new EmptyVersioningDictionary(compareFunction);
        }

        public abstract VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue);

        public abstract VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove);

        public TValue Get(TKey key)
        {
            TValue value;
            if (this.TryGetValue(key, out value))
            {
                return value;
            }

            throw new KeyNotFoundException(key.ToString());
        }

        public abstract bool TryGetValue(TKey key, out TValue value);

        internal sealed class EmptyVersioningDictionary : VersioningDictionary<TKey, TValue>
        {
            public EmptyVersioningDictionary(Func<TKey, TKey, int> compareFunction)
                : base(compareFunction)
            {
            }

            public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue)
            {
                return new OneKeyDictionary(this.CompareFunction, keyToSet, newValue);
            }

            public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove)
            {
                throw new KeyNotFoundException(keyToRemove.ToString());
            }

            public override bool TryGetValue(TKey key, out TValue value)
            {
                value = default(TValue);
                return false;
            }
        }

        internal sealed class OneKeyDictionary : VersioningDictionary<TKey, TValue>
        {
            private readonly TKey key;
            private readonly TValue value;

            public OneKeyDictionary(Func<TKey, TKey, int> compareFunction, TKey key, TValue value)
                : base(compareFunction)
            {
                this.key = key;
                this.value = value;
            }

            public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue)
            {
                if (this.CompareFunction(keyToSet, this.key) == 0)
                {
                    // Replacing the single key produces a new dictionary.
                    return new OneKeyDictionary(this.CompareFunction, keyToSet, newValue);
                }

                return new TwoKeyDictionary(this.CompareFunction, this.key, this.value, keyToSet, newValue);
            }

            public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove)
            {
                if (this.CompareFunction(keyToRemove, this.key) == 0)
                {
                    return new EmptyVersioningDictionary(this.CompareFunction);
                }

                throw new KeyNotFoundException(keyToRemove.ToString());
            }

            public override bool TryGetValue(TKey key, out TValue value)
            {
                if (this.CompareFunction(key, this.key) == 0)
                {
                    value = this.value;
                    return true;
                }

                value = default(TValue);
                return false;
            }
        }

        internal sealed class TwoKeyDictionary : VersioningDictionary<TKey, TValue>
        {
            private readonly TKey firstKey;
            private readonly TValue firstValue;
            private readonly TKey secondKey;
            private readonly TValue secondValue;

            public TwoKeyDictionary(Func<TKey, TKey, int> compareFunction, TKey firstKey, TValue firstValue, TKey secondKey, TValue secondValue)
                : base(compareFunction)
            {
                this.firstKey = firstKey;
                this.firstValue = firstValue;
                this.secondKey = secondKey;
                this.secondValue = secondValue;
            }

            public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue)
            {
                if (this.CompareFunction(keyToSet, this.firstKey) == 0)
                {
                    return new TwoKeyDictionary(this.CompareFunction, keyToSet, newValue, this.secondKey, this.secondValue);
                }

                if (this.CompareFunction(keyToSet, this.secondKey) == 0)
                {
                    return new TwoKeyDictionary(this.CompareFunction, this.firstKey, this.firstValue, keyToSet, newValue);
                }

                return new TreeDictionary(this.CompareFunction, this.firstKey, this.firstValue, this.secondKey, this.secondValue, keyToSet, newValue);
            }

            public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove)
            {
                if (this.CompareFunction(keyToRemove, this.firstKey) == 0)
                {
                    return new OneKeyDictionary(this.CompareFunction, this.secondKey, this.secondValue);
                }

                if (this.CompareFunction(keyToRemove, this.secondKey) == 0)
                {
                    return new OneKeyDictionary(this.CompareFunction, this.firstKey, this.firstValue);
                }

                throw new KeyNotFoundException(keyToRemove.ToString());
            }

            public override bool TryGetValue(TKey key, out TValue value)
            {
                if (this.CompareFunction(key, this.firstKey) == 0)
                {
                    value = this.firstValue;
                    return true;
                }

                if (this.CompareFunction(key, this.secondKey) == 0)
                {
                    value = this.secondValue;
                    return true;
                }

                value = default(TValue);
                return false;
            }
        }

        internal sealed class TreeDictionary : VersioningDictionary<TKey, TValue>
        {
            private const int MaxTreeHeight = 10;
            private readonly VersioningTree<TKey, TValue> tree;

            public TreeDictionary(Func<TKey, TKey, int> compareFunction, TKey firstKey, TValue firstValue, TKey secondKey, TValue secondValue, TKey thirdKey, TValue thirdValue)
                : base(compareFunction)
            {
                this.tree = new VersioningTree<TKey, TValue>(firstKey, firstValue, null, null).SetKeyValue(secondKey, secondValue, this.CompareFunction).SetKeyValue(thirdKey, thirdValue, this.CompareFunction);
            }

            public TreeDictionary(Func<TKey, TKey, int> compareFunction, VersioningTree<TKey, TValue> tree)
                : base(compareFunction)
            {
                this.tree = tree;
            }

            public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue)
            {
                if (this.tree.Height > MaxTreeHeight)
                {
                    return new HashTreeDictionary(this.CompareFunction, this.tree, keyToSet, newValue);
                }

                return new TreeDictionary(this.CompareFunction, this.tree.SetKeyValue(keyToSet, newValue, this.CompareFunction));
            }

            public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove)
            {
                return new TreeDictionary(this.CompareFunction, this.tree.Remove(keyToRemove, this.CompareFunction));
            }

            public override bool TryGetValue(TKey key, out TValue value)
            {
                if (this.tree == null)
                {
                    value = default(TValue);
                    return false;
                }

                return this.tree.TryGetValue(key, this.CompareFunction, out value);
            }
        }

        internal sealed class HashTreeDictionary : VersioningDictionary<TKey, TValue>
        {
            private const int HashSize = 17;
            private readonly VersioningTree<TKey, TValue>[] treeBuckets;

            public HashTreeDictionary(Func<TKey, TKey, int> compareFunction, VersioningTree<TKey, TValue> tree, TKey key, TValue value)
                : base(compareFunction)
            {
                this.treeBuckets = new VersioningTree<TKey, TValue>[HashSize];
                this.SetKeyValues(tree);
                this.SetKeyValue(key, value);
            }

            public HashTreeDictionary(Func<TKey, TKey, int> compareFunction, VersioningTree<TKey, TValue>[] trees, TKey key, TValue value)
                : base(compareFunction)
            {
                this.treeBuckets = (VersioningTree<TKey, TValue>[])trees.Clone();
                this.SetKeyValue(key, value);
            }

            public HashTreeDictionary(Func<TKey, TKey, int> compareFunction, VersioningTree<TKey, TValue>[] trees, TKey key)
                : base(compareFunction)
            {
                this.treeBuckets = (VersioningTree<TKey, TValue>[])trees.Clone();
                this.RemoveKey(key);
            }

            public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue)
            {
                return new HashTreeDictionary(this.CompareFunction, this.treeBuckets, keyToSet, newValue);
            }

            public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove)
            {
                return new HashTreeDictionary(this.CompareFunction, this.treeBuckets, keyToRemove);
            }

            public override bool TryGetValue(TKey key, out TValue value)
            {
                VersioningTree<TKey, TValue> tree = this.treeBuckets[GetBucket(key)];

                if (tree == null)
                {
                    value = default(TValue);
                    return false;
                }
                else
                {
                    return tree.TryGetValue(key, this.CompareFunction, out value);
                }
            }

            private void SetKeyValue(TKey keyToSet, TValue newValue)
            {
                int hashBucket = GetBucket(keyToSet);

                if (this.treeBuckets[hashBucket] == null)
                {
                    this.treeBuckets[hashBucket] = new VersioningTree<TKey, TValue>(keyToSet, newValue, null, null);
                }
                else
                {
                    this.treeBuckets[hashBucket] = this.treeBuckets[hashBucket].SetKeyValue(keyToSet, newValue, this.CompareFunction);
                }
            }

            private void SetKeyValues(VersioningTree<TKey, TValue> tree)
            {
                if (tree == null)
                {
                    return;
                }

                this.SetKeyValue(tree.Key, tree.Value);

                this.SetKeyValues(tree.LeftChild);
                this.SetKeyValues(tree.RightChild);
            }

            private void RemoveKey(TKey keyToRemove)
            {
                int hashBucket = GetBucket(keyToRemove);

                if (this.treeBuckets[hashBucket] == null)
                {
                    throw new KeyNotFoundException(keyToRemove.ToString());
                }
                else
                {
                    this.treeBuckets[hashBucket] = this.treeBuckets[hashBucket].Remove(keyToRemove, this.CompareFunction);
                }
            }

            private static int GetBucket(TKey key)
            {
                int hash = key.GetHashCode();
                if (hash < 0)
                {
                    hash = -hash;
                }

                return hash % HashSize;
            }
        }
    }
}
