//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralPrefixesStore.cs" company="Microsoft">
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
    /// Thread-safe, per-<see cref="IEdmModel"/> store for custom URI literal prefixes.
    /// Each registered prefix maps (ordinal, case-sensitive) to an <see cref="IEdmTypeReference"/> used by the URI parser
    /// to interpret prefix-qualified literal values.
    /// </summary>
    /// <remarks>
    /// Concurrency guarantees:
    /// 1) <c>map</c> is an immutable dictionary reference; readers always see a fully-formed snapshot.
    /// 2) All mutations replace <c>map</c> atomically via a CAS loop (<see cref="CompareExchangeLoop{T}"/>).
    /// 3) A single store instance is created per model and cached as a direct-value annotation; creation is serialized
    ///    with a per‑model lock (held in a <see cref="ConditionalWeakTable{TKey, TValue}"/> to avoid leaking).
    /// These guarantees allow lock-free reads (<see cref="TryGet"/> / <see cref="Snapshot"/>)
    /// and contention-minimized writes (<see cref="Add"/> / <see cref="Remove"/>).
    /// </remarks>
    internal sealed class CustomUriLiteralPrefixesStore
    {
        private ImmutableDictionary<string, IEdmTypeReference> map =
            ImmutableDictionary.Create<string, IEdmTypeReference>(StringComparer.Ordinal);

        // One private gate per model instance to serialize creation of the store.
        // Using CWT keeps the gate's lifetime tied to the model (no leaks) and
        // avoids global contention across different models.
        private static readonly ConditionalWeakTable<IEdmModel, object> lockPerModel = new ConditionalWeakTable<IEdmModel, object>();

        // Only creatable through GetOrCreate
        private CustomUriLiteralPrefixesStore()
        {
        }

        /// <summary>
        /// Gets the store instance associated with <paramref name="model"/>, creating and annotating it if it does not yet exist.
        /// A per‑model lock ensures only one instance is created under concurrent access.
        /// </summary>
        /// <param name="model">The Edm model for which to obtain the store.</param>
        /// <returns>The existing or newly created <see cref="CustomUriLiteralPrefixesStore"/>.</returns>
        public static CustomUriLiteralPrefixesStore GetOrCreate(IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, nameof(model));

            CustomUriLiteralPrefixesStore existing = model.GetAnnotationValue<CustomUriLiteralPrefixesStore>(model);
            if (existing != null)
            {
                return existing;
            }

            object gate = lockPerModel.GetValue(model, _ => new object());
            lock (gate)
            {
                // Try to get the typed direct-value annotation from the model element
                existing = model.GetAnnotationValue<CustomUriLiteralPrefixesStore>(model);
                if (existing != null)
                {
                    // Another thread already created the store while we were waiting for the lock
                    return existing;
                }

                CustomUriLiteralPrefixesStore created = new CustomUriLiteralPrefixesStore();
                model.SetAnnotationValue(model, created);

                return created;
            }
        }

        /// <summary>
        /// Returns an immutable, point‑in‑time snapshot of the current prefix mapping.
        /// </summary>
        /// <remarks>
        /// Thread safety: Safe to call concurrently. The returned dictionary is immutable; subsequent updates
        /// to the store are not reflected unless <see cref="Snapshot()"/> is called again.
        /// </remarks>
        public IReadOnlyDictionary<string, IEdmTypeReference> Snapshot() => this.map;

        /// <summary>
        /// Attempts to resolve a registered literal prefix to its associated Edm type.
        /// </summary>
        /// <param name="literalPrefix">The prefix to look up (ordinal, case-sensitive).</param>
        /// <param name="edmType">When successful, receives the associated type; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if the prefix is registered; otherwise <c>false</c>.</returns>
        public bool TryGet(string literalPrefix, out IEdmTypeReference edmType)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, nameof(literalPrefix));

            return this.map.TryGetValue(literalPrefix, out edmType);
        }

        /// <summary>
        /// Registers a new literal prefix mapping if it does not already exist.
        /// </summary>
        /// <param name="literalPrefix">The prefix to register (ordinal, case-sensitive).</param>
        /// <param name="edmType">The Edm type the prefix represents.</param>
        /// <returns><c>true</c> if the prefix was added; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Thread safety: Implemented via an immutable snapshot + CAS replacement; no global locks required.
        /// </remarks>
        public bool Add(string literalPrefix, IEdmTypeReference edmType)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, nameof(literalPrefix));
            ExceptionUtils.CheckArgumentNotNull(edmType, nameof(edmType));

            return CompareExchangeLoop(
                ref this.map,
                snapshot =>
                {
                    if (snapshot.ContainsKey(literalPrefix))
                    {
                        return null; // no change
                    }

                    return snapshot.Add(literalPrefix, edmType);
                }) != null;
        }

        /// <summary>
        /// Unregisters a previously added literal prefix.
        /// </summary>
        /// <param name="literalPrefix">The prefix to remove.</param>
        /// <returns><c>true</c> if the prefix existed and was removed; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Thread safety: Uses immutable snapshot + CAS replacement to publish updates atomically.
        /// </remarks>
        public bool Remove(string literalPrefix)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, nameof(literalPrefix));

            return CompareExchangeLoop(
                ref this.map,
                snapshot =>
                {
                    if (!snapshot.ContainsKey(literalPrefix))
                    {
                        return null; // no change
                    }

                    return snapshot.Remove(literalPrefix);
                }) != null;
        }

        /// <summary>
        /// Repeatedly applies <paramref name="updateFunc"/> to the current reference until an atomic compare–exchange succeeds
        /// or <paramref name="updateFunc"/> signals no update (by returning <c>null</c>).
        /// </summary>
        /// <typeparam name="T">The reference type being updated.</typeparam>
        /// <param name="location">The reference to update.</param>
        /// <param name="updateFunc">A function that takes the current value and returns the updated value, or <c>null</c> to indicate no update.</param>
        /// <returns>The updated value if the update succeeded; otherwise, <c>null</c>.</returns>
        private static T CompareExchangeLoop<T>(ref T location, Func<T, T> updateFunc) where T : class
        {
            while (true)
            {
                var snapshot = location;
                var updated = updateFunc(snapshot);
                if (updated == null)
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
