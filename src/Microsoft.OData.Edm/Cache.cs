//---------------------------------------------------------------------
// <copyright file="Cache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Provides a caching mechanism for semantic properties.
    /// </summary>
    /// <typeparam name="TContainer">Type of the element that contains the cached property</typeparam>
    /// <typeparam name="TProperty">Type of the cached property</typeparam>
    //// Using a Cache requires a function to compute the cached value at first access and a function
    //// to create a result when the computation of the value involves a cyclic dependency. (The second
    //// function can be omitted if a cycle is impossible.)
    ////
    //// Cache provides concurrency safety.
    internal class Cache<TContainer, TProperty>
    {
        private object value = CacheHelper.Unknown;

        // In order to detect the boundaries of a cycle, we use two sentinel values. When we encounter the first sentinel, we know that a cycle exists and that we are a point on that cycle.
        // When we reach an instance of the second sentinel, we know we have made a complete circuit of the cycle and that every node in the cycle has been marked with the second sentinel.
        public TProperty GetValue(TContainer container, Func<TContainer, TProperty> compute, Func<TContainer, TProperty> onCycle)
        {
            // If a cycle is present, locking on the cache object can produce a deadlock (if one thread asks for one node in the cycle and another
            // thread asks for another). If a cycle is possible, onCycle is required to be nonnull and the same--because there is one instance per
            // property per TContainer type--for all participants in the cycle. Locking on onCycle therefore locks out all other computations of
            // the property for all instances of TContainer, which by definition includes all participants in a cycle. If no cycle is possible,
            // locking on the cache object allows computation of the property for other instances to occur in parallel and so is minimally selfish.
            object lockOn = (object)onCycle ?? this;
            object result = this.value;

            if (result == CacheHelper.Unknown)
            {
                lock (lockOn)
                {
                    // If another thread computed a value, use that. If the value is still unknown after aquiring the lock, compute the value.
                    if (this.value == CacheHelper.Unknown)
                    {
                        this.value = CacheHelper.CycleSentinel;
                        TProperty computedValue;
                        try
                        {
                            computedValue = compute(container);
                        }
                        catch
                        {
                            this.value = CacheHelper.Unknown;
                            throw;
                        }

                        // If this.value changed during computation, this cache was involved in a cycle and this.value was already set to the onCycle value
                        Debug.Assert(this.value != CacheHelper.SecondPassCycleSentinel, "Cycles should already have their cycle value set");
                        if (this.value == CacheHelper.CycleSentinel)
                        {
                            this.value = typeof(TProperty) == typeof(bool) ? CacheHelper.BoxedBool((bool)(object)computedValue) : computedValue;
                        }
                    }

                    result = this.value;
                }
            }
            else if (result == CacheHelper.CycleSentinel)
            {
                lock (lockOn)
                {
                    // If another thread computed a value, use that. If the value is still a sentinel after acquiring the lock,
                    // by definition this thread is the one computing the value. (Otherwise, the lock taken when the value was
                    // Unknown would still be in force.)
                    if (this.value == CacheHelper.CycleSentinel)
                    {
                        this.value = CacheHelper.SecondPassCycleSentinel;
                        try
                        {
                            compute(container);
                        }
                        catch
                        {
                            this.value = CacheHelper.CycleSentinel;
                            throw;
                        }

                        if (this.value == CacheHelper.SecondPassCycleSentinel)
                        {
                            this.value = onCycle(container);
                        }
                    }
                    else if (this.value == CacheHelper.Unknown)
                    {
                        // Another thread cleared the cache.
                        return this.GetValue(container, compute, onCycle);
                    }

                    result = this.value;
                }
            }
            else if (result == CacheHelper.SecondPassCycleSentinel)
            {
                lock (lockOn)
                {
                    // If another thread computed a value, use that. If the value is still a sentinel after acquiring the lock,
                    // by definition this thread is the one computing the value. (Otherwise, the lock taken when the value was
                    // Unknown would still be in force.)
                    if (this.value == CacheHelper.SecondPassCycleSentinel)
                    {
                        this.value = onCycle(container);
                    }
                    else if (this.value == CacheHelper.Unknown)
                    {
                        // Another thread cleared the cache.
                        return this.GetValue(container, compute, onCycle);
                    }

                    result = this.value;
                }
            }

            return (TProperty)result;
        }

        public void Clear(Func<TContainer, TProperty> onCycle)
        {
            lock ((object)onCycle ?? this)
            {
                if (this.value != CacheHelper.CycleSentinel && this.value != CacheHelper.SecondPassCycleSentinel)
                {
                    this.value = CacheHelper.Unknown;
                }
            }
        }
    }
}