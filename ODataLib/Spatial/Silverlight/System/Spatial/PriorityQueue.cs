//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace System.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Queue where things are seen in highest priority (highest compare) order 
    /// </summary>
    /// <typeparam name="TValue">The type of the values stored in priority order</typeparam>
    internal class PriorityQueue<TValue>
    {
        /// <summary>
        /// The list of queued items.
        /// This is non-generic to avoid issues with the NetCF's reflection stack.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Assembly will not be pre-compiled")]
        private readonly List<KeyValuePair<double, object>> data = new List<KeyValuePair<double, object>>();

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class
        /// DEVNOTE: this is only here for the FxCop suppression.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Assembly will not be pre-compiled")]
        public PriorityQueue()
        {
        }

        /// <summary>
        /// Gets the number of items in the queue
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Assembly will not be pre-compiled")]
        public int Count
        {
            get { return data.Count; }
        }

        /// <summary>
        /// Returns the top queue value without removing it.
        /// </summary>
        /// <returns>The top value of the queue</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Assembly will not be pre-compiled")]
        public TValue Peek()
        {
            if (data.Count == 0)
            {
                throw new InvalidOperationException(Strings.PriorityQueueOperationNotValidOnEmptyQueue);
            }

            return (TValue)this.data[0].Value;
        }

        /// <summary>
        /// Adds a new value to the queue by priority.
        /// </summary>
        /// <param name="priority">The priority of the new item to add.</param>
        /// <param name="value">The new item being added.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Assembly will not be pre-compiled")]
        public void Enqueue(double priority, TValue value)
        {
            data.Add(new KeyValuePair<double, object>(priority, value));

            // maintain high-to-low sorting
            data.Sort((lhs, rhs) => -lhs.Key.CompareTo(rhs.Key));
        }

        /// <summary>
        /// Returns a value indicating whether there is already an item with the given priority in the queue
        /// </summary>
        /// <param name="priority">The priority to check</param>
        /// <returns>Whether or not an item with the given priority is in the queue</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Assembly will not be pre-compiled")]
        public bool Contains(double priority)
        {
            return data.Any(v => v.Key == priority);
        }

        /// <summary>
        /// Removes the item with the priority specified from the queue
        /// </summary>
        /// <param name="priority">The priority of the item to be removed from the queue</param>
        /// <returns>The value of the removed item.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Assembly will not be pre-compiled")]
        public TValue DequeueByPriority(double priority)
        {
            foreach (var pair in data)
            {
                if (pair.Key == priority)
                {
                    data.Remove(pair);
                    return (TValue)pair.Value;
                }
            }

            throw new InvalidOperationException(Strings.PriorityQueueDoesNotContainItem(priority));
        }
    }
}
