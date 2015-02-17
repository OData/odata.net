//---------------------------------------------------------------------
// <copyright file="WeightedCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// ckass WeightedCollection
    /// </summary>
    /// <typeparam name="TItem">item type</typeparam>
    public class WeightedCollection<TItem> : IEnumerable<TItem>
    {
        /// <summary>
        /// The weightlist
        /// </summary>
        private readonly List<int> weightList = new List<int>();

        /// <summary>
        /// The init locker
        /// </summary>
        private readonly object initLocker = new object();

        /// <summary>
        /// The initialized
        /// </summary>
        private bool initalized;

        /// <summary>
        /// The Index
        /// </summary>
        private int index = -1;

        /// <summary>
        /// Initializes a new instance of the WeightedCollection class
        /// </summary>
        public WeightedCollection()
        {
            this.Items = new List<TItem>();
        }

        /// <summary>
        /// The constriants
        /// </summary>
        public event Func<TItem, bool> Constraints;

        /// <summary>
        /// Gets item list
        /// </summary>
        public List<TItem> Items { get; private set; }

        /// <summary>
        /// Get next item
        /// </summary>
        /// <returns>The item</returns>
        public TItem GetNext()
        {
            if (!this.initalized)
            {
                this.Init();
            }

            int i = Interlocked.Increment(ref this.index);
            return this.Items[this.weightList[i % this.weightList.Count]];
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return this.Items.Where(item => this.Constraints == null || this.Constraints(item)).GetEnumerator();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>the enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Randomize the collection
        /// </summary>
        public void Randomize()
        {
            this.weightList.Randomize();
        }

        /// <summary>
        /// Add a item
        /// </summary>
        /// <param name="builder">the item</param>
        /// <param name="weight">the weight</param>
        public void Add(TItem builder, int weight)
        {
            int id = this.Items.Count;
            this.Items.Add(builder);
            for (int i = 0; i < weight; i++)
            {
                this.weightList.Add(id);
            }
        }

        /// <summary>
        /// Add a item
        /// </summary>
        /// <param name="builder">the item</param>
        public void Add(TItem builder)
        {
            this.Add(builder, 1);
        }

        /// <summary>
        /// Add a range of item
        /// </summary>
        /// <param name="builders">the items</param>
        public void AddRange(IEnumerable<TItem> builders)
        {
            foreach (TItem qb in builders)
            {
                Add(qb);
            }
        }

        /// <summary>
        /// Mix two collections
        /// </summary>
        /// <typeparam name="TItem2">Item2 type</typeparam>
        /// <param name="mixWith">mixwith collection</param>
        /// <returns>Mixed collection</returns>
        public WeightedCollection<Tuple<TItem, TItem2>> Mix<TItem2>(WeightedCollection<TItem2> mixWith)
        {
            return Mix(mixWith, Tuple.Create);
        }

        /// <summary>
        /// Mix two collection
        /// </summary>
        /// <typeparam name="TItem2">Item2 type</typeparam>
        /// <typeparam name="TResult">Return item type</typeparam>
        /// <param name="mixWith">Mix collection</param>
        /// <param name="transform">The Transform</param>
        /// <returns>Mixed collection</returns>
        public WeightedCollection<TResult> Mix<TItem2, TResult>(
            WeightedCollection<TItem2> mixWith, Func<TItem, TItem2, TResult> transform)
        {
            this.Init();
            mixWith.Init();
            var ret = new WeightedCollection<TResult>();
            foreach (var w1 in this.weightList.GroupBy(w => w))
            {
                foreach (var w2 in mixWith.weightList.GroupBy(w => w))
                {
                    ret.Add(transform(this.Items[w1.Key], mixWith.Items[w2.Key]), w1.Count() * w2.Count());
                }
            }

            return ret;
        }

        /// <summary>
        /// Union two collections
        /// </summary>
        /// <param name="unionWith">unit collection</param>
        /// <returns>union collection</returns>
        public WeightedCollection<TItem> Union(WeightedCollection<TItem> unionWith)
        {
            this.Init();
            unionWith.Init();
            var ret = new WeightedCollection<TItem>();
            foreach (var w1 in this.weightList.GroupBy(w => w))
            {
                ret.Add(this.Items[w1.Key], w1.Count());
            }

            foreach (var w1 in unionWith.weightList.GroupBy(w => w))
            {
                ret.Add(unionWith.Items[w1.Key], w1.Count());
            }

            return ret;
        }

        /// <summary>
        /// Init collection
        /// </summary>
        private void Init()
        {
            lock (this.initLocker)
            {
                if (!this.initalized)
                {
                    if (this.Constraints != null)
                    {
                        for (int i = 0; i < this.Items.Count; i++)
                        {
                            if (!this.Constraints(this.Items[i]))
                            {
                                this.weightList.Remove(i);
                            }
                        }
                    }

                    this.Randomize();
                    this.initalized = true;
                }
            }
        }
    }
}