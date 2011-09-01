//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Diagnostics;

namespace Microsoft.Data.Edm.Internal
{
    /// <summary>
    /// Provides a caching mechanism for semantic properties.
    /// </summary>
    /// <typeparam name="TContainer">Type of the element that contains the cached property</typeparam>
    /// <typeparam name="TProperty">Type of the cached property</typeparam>
    // Using a Cache requires a function to compute the cached value at first access and a function
    // to create a result when the computation of the value involves a cyclic dependency. (The second
    // function can be omitted if a cycle is impossible.)
    //
    // Cache provides concurrency safety (NYI) and so is preferred to ad hoc caching.
    internal class Cache<TContainer, TProperty>
    {
        private object value = CacheHelper.Unknown;

        public bool HasValue
        {
            get { return this.value != CacheHelper.Unknown; }
        }

        // In order to detect the boundaries of a cycle, we use two sentinel values. When we encounter the first sentinel, we know that a cycle exists and that we are a point on that cycle.
        // When we reach an instance of the second sentinel, we know we have made a complete circuit of the cycle and that every node in the cycle has been marked with the second sentinel.
        public TProperty GetValue(TContainer container, Func<TContainer, TProperty> compute, Func<TContainer, TProperty> onCycle)
        {
            if (this.value == CacheHelper.Unknown)
            {
                this.value = CacheHelper.CycleSentinel;
                TProperty computedValue = compute(container);
                // If this.value changed during computation, this cache was involved in a cycle and this.value was already set to the onCycle value
                Debug.Assert(this.value != CacheHelper.SecondPassCycleSentinel, "Cycles should already have their cycle value set");
                if (this.value == CacheHelper.CycleSentinel)
                {
                    this.value = typeof(TProperty) == typeof(bool) ? CacheHelper.BoxedBool((bool)(object)computedValue) : computedValue;
                }
            }
            else if (this.value == CacheHelper.CycleSentinel)
            {
                this.value = CacheHelper.SecondPassCycleSentinel;
                compute(container);
                if (this.value == CacheHelper.SecondPassCycleSentinel)
                {
                    this.value = onCycle(container);
                }
            }
            else if (this.value == CacheHelper.SecondPassCycleSentinel)
            {
                this.value = onCycle(container);
            }

            return (TProperty)this.value;
        }

        public void Clear()
        {
            if (this.value != CacheHelper.CycleSentinel && this.value != CacheHelper.SecondPassCycleSentinel)
            {
                this.value = CacheHelper.Unknown;
            }
        }
    }
}
