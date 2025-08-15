//---------------------------------------------------------------------
// <copyright file="CustomUriFunctionsStore.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Thread-safe store for managing custom URI function signatures associated with an Edm model.
    /// This class allows adding, removing, and retrieving custom function signatures for use in OData URI parsing.
    /// </summary>
    /// <remarks>
    /// Invariants:
    /// 1) <c>map</c> is an immutable dictionary instance published atomically.
    /// 2) Each value is a list that is treated as immutable once stored in <c>map</c>
    ///    (never mutate in place; always clone, modify, then publish via CAS).
    /// 3) All updates occur inside <c>CompareExchangeLoop</c>.
    /// These invariants allow lock‑free, thread‑safe publication and consistent snapshots.
    /// </remarks>
    internal sealed class CustomUriFunctionsStore
    {
        // Invariant: Lists stored in 'map' are treated as immutable after publication.
        // All updates create a new List<T> and install it via a new ImmutableDictionary using CAS.
        // Do NOT mutate any list retrieved from 'map'.
        private ImmutableDictionary<string, List<FunctionSignatureWithReturnType>> map
            = ImmutableDictionary<string, List<FunctionSignatureWithReturnType>>.Empty
            .WithComparers(StringComparer.Ordinal);

        // One private gate per model instance to serialize creation of the store.
        // Using CWT keeps the gate's lifetime tied to the model (no leaks) and
        // avoids global contention across different models.
        private static readonly ConditionalWeakTable<IEdmModel, object> lockPerModel = new ConditionalWeakTable<IEdmModel, object>();

        // Store can only be created via GetOrCreate
        private CustomUriFunctionsStore() { }

        /// <summary>
        /// Retrieves the <see cref="CustomUriFunctionsStore"/> instance associated with the given Edm model,
        /// creating and annotating it on the model if it does not already exist.
        /// </summary>
        /// <param name="model">The Edm model to associate with the store.</param>
        /// <returns>The <see cref="CustomUriFunctionsStore"/> instance for the model.</returns>
        public static CustomUriFunctionsStore GetOrCreate(IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            // Try to get the typed direct-value annotation from the model element
            CustomUriFunctionsStore existing = model.GetAnnotationValue<CustomUriFunctionsStore>(model);
            if (existing != null)
            {
                return existing;
            }

            object gate = lockPerModel.GetValue(model, _ => new object());
            lock (gate)
            {
                existing = model.GetAnnotationValue<CustomUriFunctionsStore>(model);
                if (existing != null)
                {
                    // Another thread already created the store while we were waiting for the lock
                    return existing;
                }

                CustomUriFunctionsStore created = new CustomUriFunctionsStore();
                model.SetAnnotationValue(model, created);

                return created;
            }
        }

        /// <summary>
        /// Returns a snapshot of all custom URI function signatures currently stored.
        /// </summary>
        /// <returns>
        /// A read-only dictionary mapping function names to their corresponding read-only lists of signatures.
        /// </returns>
        public IReadOnlyDictionary<string, IReadOnlyList<FunctionSignatureWithReturnType>> Snapshot()
        {
            ImmutableDictionary<string, List<FunctionSignatureWithReturnType>> current = this.map;
            Dictionary<string, IReadOnlyList<FunctionSignatureWithReturnType>> copy =
                new Dictionary<string, IReadOnlyList<FunctionSignatureWithReturnType>>(current.Count, current.KeyComparer);

            foreach (KeyValuePair<string, List<FunctionSignatureWithReturnType>> kvp in current)
            {
                copy[kvp.Key] = kvp.Value.AsReadOnly();
            }

            return copy;
        }

        /// <summary>
        /// Attempts to retrieve all overloads of a custom URI function with the specified name.
        /// </summary>
        /// <param name="functionName">The name of the custom function to retrieve.</param>
        /// <param name="ignoreCase">Whether to ignore case when matching the function name.</param>
        /// <param name="functionSignatures">
        /// When this method returns, contains the list of function signatures if found; otherwise, null.
        /// </param>
        /// <returns><c>true</c> if one or more overloads were found; otherwise, <c>false</c>.</returns>
        public bool TryGet(string functionName, bool ignoreCase, out IReadOnlyList<FunctionSignatureWithReturnType> functionSignatures)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");

            functionSignatures = null;

            if (!ignoreCase)
            {
                if (this.map.TryGetValue(functionName, out List<FunctionSignatureWithReturnType> signatures))
                {
                    functionSignatures = signatures.AsReadOnly();

                    return true;
                }

                return false;
            }

            StringComparer comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            foreach (KeyValuePair<string, List<FunctionSignatureWithReturnType>> kvp in this.map)
            {
                if (comparer.Equals(kvp.Key, functionName))
                {
                    functionSignatures = kvp.Value.AsReadOnly();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a custom URI function signature for the specified function name.
        /// If the function name already exists, the signature is added as an overload if it is not already present.
        /// </summary>
        /// <param name="functionName">The name of the custom function.</param>
        /// <param name="functionSignature">The function signature to add.</param>
        /// <remarks>
        /// Thread‑safety: This method updates <c>map</c> via a CAS‑based CompareExchange loop.
        /// The dictionary is treated as immutable and replaced atomically; the per‑key lists
        /// are also treated as immutable once published (copy‑on‑write only). Do not mutate a
        /// list instance after it is stored in <c>map</c>.
        /// Performance: When appending, we pre‑size the new list to <c>Count + 1</c> to avoid
        /// internal resizing/allocations. We also avoid calling <c>SetItem</c> if no change
        /// is required.
        /// </remarks>
        public void Add(string functionName, FunctionSignatureWithReturnType functionSignature)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");
            ExceptionUtils.CheckArgumentNotNull(functionSignature, "functionSignature");

            CompareExchangeLoop(
                ref this.map,
                snapshot =>
                {
                    if (!snapshot.TryGetValue(functionName, out List<FunctionSignatureWithReturnType> existingSignatures))
                    {
                        // We create the list with a capacity of 1 because don't append to this instance later;
                        // it will be replaced on the next update.
                        return snapshot.Add(functionName, new List<FunctionSignatureWithReturnType>(1) { functionSignature });
                    }
                    else
                    {
                        if (!existingSignatures.Contains(functionSignature))
                        {
                            // NOTE: We treat the snapshot's list as immutable. Pre‑size to Count + 1 so the
                            // subsequent Add won't cause an internal resize. Using a collection expression
                            // (e.g., [..existingSignatures, functionSignature]) may not always pre‑size,
                            // depending on compiler/runtime optimizations. This explicit capacity avoids
                            // extra allocations on this hot path.
                            List<FunctionSignatureWithReturnType> updatedSignatures = new List<FunctionSignatureWithReturnType>(existingSignatures.Count + 1);

                            updatedSignatures.AddRange(existingSignatures);
                            updatedSignatures.Add(functionSignature);

                            return snapshot.SetItem(functionName, updatedSignatures);
                        }

                        // No update required -> null will signal CompareExchangeLoop to exit without CAS
                        return null;
                    }
                });
        }

        /// <summary>
        /// Removes a specific overload (signature) of a custom URI function.
        /// </summary>
        /// <param name="functionName">The name of the custom function.</param>
        /// <param name="functionSignature">The function signature to remove.</param>
        /// <returns><c>true</c> if the signature was found and removed; otherwise, <c>false</c>.</returns>
        public bool Remove(string functionName, FunctionSignatureWithReturnType functionSignature)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");
            ExceptionUtils.CheckArgumentNotNull(functionSignature, "functionSignature");

            return CompareExchangeLoop(
                ref this.map,
                snapshot =>
                {
                    if (!snapshot.TryGetValue(functionName, out List<FunctionSignatureWithReturnType> existingSignatures))
                    {
                        return null;
                    }

                    List<FunctionSignatureWithReturnType> updatedSignatures = new List<FunctionSignatureWithReturnType>(existingSignatures);
                    if (!updatedSignatures.Remove(functionSignature))
                    {
                        return null;
                    }

                    if (updatedSignatures.Count == 0)
                    {
                        return snapshot.Remove(functionName);
                    }
                    else
                    {
                        return snapshot.SetItem(functionName, updatedSignatures);
                    }
                }) != null;
        }

        /// <summary>
        /// Removes all overloads of a custom URI function with the specified name.
        /// </summary>
        /// <param name="functionName">The name of the custom function to remove.</param>
        /// <returns><c>true</c> if the function was found and removed; otherwise, <c>false</c>.</returns>
        public bool Remove(string functionName)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");

            return CompareExchangeLoop(
                ref this.map,
                snapshot => snapshot.ContainsKey(functionName) ? snapshot.Remove(functionName) : null) != null;
        }

        /// <summary>
        /// Atomically updates the specified reference using the provided update function until the update succeeds.
        /// </summary>
        /// <typeparam name="T">The reference type being updated.</typeparam>
        /// <param name="location">The reference to update.</param>
        /// <param name="updateFunc">A function that takes the current value and returns the updated value, or null to indicate no update.</param>
        /// <returns>The updated value if the update succeeded; otherwise, null.</returns>
        private static T CompareExchangeLoop<T>(ref T location, Func<T, T> updateFunc) where T : class
        {
            while (true)
            {
                T snapshot = location;
                T updated = updateFunc(snapshot);
                if (updated == null) // Indicates no change / failed
                {
                    return null;
                }

                if (Interlocked.CompareExchange(ref location, updated, snapshot) == snapshot)
                {
                    return updated;
                }
            }
        }
    }
}
