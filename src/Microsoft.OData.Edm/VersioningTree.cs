//---------------------------------------------------------------------
// <copyright file="VersioningTree.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Provides an approximately-balanced binary search tree that is thread safe by virtue of being immutable.
    /// Updates return a new tree (which, for efficiency, may share some state with the old one).
    /// </summary>
    /// <typeparam name="TKey">Key type of the tree.</typeparam>
    /// <typeparam name="TValue">Value type of the tree.</typeparam>
    internal class VersioningTree<TKey, TValue>
    {
        public readonly TKey Key;
        public readonly TValue Value;
        public readonly int Height;
        public readonly VersioningTree<TKey, TValue> LeftChild;
        public readonly VersioningTree<TKey, TValue> RightChild;

        /// <summary>
        /// Initializes a new instance of VersioningTree.
        /// </summary>
        /// <param name="key">The key of the tree node.</param>
        /// <param name="value">The value of the tree node.</param>
        /// <param name="leftChild">A tree with all keys less than the key of the tree node. May be null.</param>
        /// <param name="rightChild">A tree with all keys greater than the key of the tree node. May be null.</param>
        public VersioningTree(TKey key, TValue value, VersioningTree<TKey, TValue> leftChild, VersioningTree<TKey, TValue> rightChild)
        {
            this.Key = key;
            this.Value = value;
            this.Height = Max(GetHeight(leftChild), GetHeight(rightChild)) + 1;
            this.LeftChild = leftChild;
            this.RightChild = rightChild;
        }

        public TValue GetValue(TKey key, Func<TKey, TKey, int> compareFunction)
        {
            TValue value;
            if (this.TryGetValue(key, compareFunction, out value))
            {
                return value;
            }

            throw new KeyNotFoundException(key.ToString());
        }

        public bool TryGetValue(TKey key, Func<TKey, TKey, int> compareFunction, out TValue value)
        {
            VersioningTree<TKey, TValue> tree = this;
            while (tree != null)
            {
                int comparison = compareFunction(key, tree.Key);
                if (comparison == 0)
                {
                    value = tree.Value;
                    return true;
                }

                tree = comparison < 0 ? tree = tree.LeftChild : tree.RightChild;
            }

            value = default(TValue);
            return false;
        }

        public VersioningTree<TKey, TValue> SetKeyValue(TKey key, TValue value, Func<TKey, TKey, int> compareFunction)
        {
            VersioningTree<TKey, TValue> leftChild = this.LeftChild;
            VersioningTree<TKey, TValue> rightChild = this.RightChild;

            int comparison = compareFunction(key, this.Key);
            if (comparison < 0)
            {
                // Insert to the left.
                if (GetHeight(leftChild) > GetHeight(rightChild))
                {
                    // The left subtree is taller than the right, so rotate to the right.
                    int leftChildComparison = compareFunction(key, leftChild.Key);

                    VersioningTree<TKey, TValue> newLeft =
                        leftChildComparison < 0 ? SetKeyValue(leftChild.LeftChild, key, value, compareFunction) : leftChild.LeftChild;
                    VersioningTree<TKey, TValue> newRight =
                        new VersioningTree<TKey, TValue>(
                            this.Key,
                            this.Value,
                            leftChildComparison > 0 ? SetKeyValue(leftChild.RightChild, key, value, compareFunction) : leftChild.RightChild,
                            rightChild);
                    return
                        new VersioningTree<TKey, TValue>(
                            leftChildComparison == 0 ? key : leftChild.Key,
                            leftChildComparison == 0 ? value : leftChild.Value,
                            newLeft,
                            newRight);
                }
                else
                {
                    return
                        new VersioningTree<TKey, TValue>(
                            this.Key,
                            this.Value,
                            SetKeyValue(leftChild, key, value, compareFunction),
                            rightChild);
                }
            }
            else if (comparison == 0)
            {
                // Insert here.
                return new VersioningTree<TKey, TValue>(key, value, leftChild, rightChild);
            }
            else
            {
                // Insert to the right.
                if (GetHeight(leftChild) < GetHeight(rightChild))
                {
                    // The right subtree is taller than the left, so rotate to the left.
                    int rightChildComparison = compareFunction(key, rightChild.Key);

                    VersioningTree<TKey, TValue> newLeft =
                         new VersioningTree<TKey, TValue>(
                            this.Key,
                            this.Value,
                            leftChild,
                            rightChildComparison < 0 ? SetKeyValue(rightChild.LeftChild, key, value, compareFunction) : rightChild.LeftChild);
                    VersioningTree<TKey, TValue> newRight =
                        rightChildComparison > 0 ? SetKeyValue(rightChild.RightChild, key, value, compareFunction) : rightChild.RightChild;
                    return
                        new VersioningTree<TKey, TValue>(
                            rightChildComparison == 0 ? key : rightChild.Key,
                            rightChildComparison == 0 ? value : rightChild.Value,
                            newLeft,
                            newRight);
                }
                else
                {
                    return
                        new VersioningTree<TKey, TValue>(
                            this.Key,
                            this.Value,
                            leftChild,
                            SetKeyValue(rightChild, key, value, compareFunction));
                }
            }
        }

        public VersioningTree<TKey, TValue> Remove(TKey key, Func<TKey, TKey, int> compareFunction)
        {
            int comparision = compareFunction(key, this.Key);
            if (comparision < 0)
            {
                // Delete to the left.
                if (this.LeftChild == null)
                {
                    throw new KeyNotFoundException(key.ToString());
                }

                return new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild.Remove(key, compareFunction), this.RightChild);
            }
            else if (comparision == 0)
            {
                // Delete here.
                if (this.LeftChild == null)
                {
                    return this.RightChild;
                }

                if (this.RightChild == null)
                {
                    return this.LeftChild;
                }

                if (this.LeftChild.Height < this.RightChild.Height)
                {
                    // Attach the right child as the rightmost child of the left child. This can unbalance the tree, but
                    // maintaining balance can cost a great deal of storage allocation.
                    return this.LeftChild.MakeRightmost(this.RightChild);
                }
                else
                {
                    // Attach the left child as the leftmost child of the right child.
                    return this.RightChild.MakeLeftmost(this.LeftChild);
                }
            }
            else
            {
                // Delete to the right.
                if (this.RightChild == null)
                {
                    throw new KeyNotFoundException(key.ToString());
                }

                return new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild, this.RightChild.Remove(key, compareFunction));
            }
        }

        private static VersioningTree<TKey, TValue> SetKeyValue(VersioningTree<TKey, TValue> me, TKey key, TValue value, Func<TKey, TKey, int> compareFunction)
        {
            if (me == null)
            {
                return new VersioningTree<TKey, TValue>(key, value, null, null);
            }

            return me.SetKeyValue(key, value, compareFunction);
        }

        private static int GetHeight(VersioningTree<TKey, TValue> tree)
        {
            return tree == null ? 0 : tree.Height;
        }

        private static int Max(int x, int y)
        {
            return x > y ? x : y;
        }

        private VersioningTree<TKey, TValue> MakeLeftmost(VersioningTree<TKey, TValue> leftmost)
        {
            if (this.LeftChild == null)
            {
                return new VersioningTree<TKey, TValue>(this.Key, this.Value, leftmost, this.RightChild);
            }

            return new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild.MakeLeftmost(leftmost), this.RightChild);
        }

        private VersioningTree<TKey, TValue> MakeRightmost(VersioningTree<TKey, TValue> rightmost)
        {
            if (this.RightChild == null)
            {
                return new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild, rightmost);
            }

            return new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild, this.RightChild.MakeRightmost(rightmost));
        }
    }
}
