//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralPrefixesStore.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Thread-safe store for managing custom URI literal prefixes associated with an Edm model.
    /// </summary>
    internal sealed class CustomUriLiteralPrefixesStore
    {
        private ImmutableDictionary<string, IEdmTypeReference> map =
            ImmutableDictionary.Create<string, IEdmTypeReference>(StringComparer.Ordinal);

        // Only creatable through GetOrCreate
        private CustomUriLiteralPrefixesStore() { }

        /// <summary>
        /// Get or create the model-scoped store via direct-value annotation.
        /// Uses a CAS loop to ensure we return the winning instance under races.
        /// </summary>
        public static CustomUriLiteralPrefixesStore GetOrCreate(IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, nameof(model));

            while (true)
            {
                var existing = model.GetAnnotationValue<CustomUriLiteralPrefixesStore>(model);
                if (existing != null)
                {
                    return existing;
                }

                var created = new CustomUriLiteralPrefixesStore();
                model.SetAnnotationValue(model, created);

                var after = model.GetAnnotationValue<CustomUriLiteralPrefixesStore>(model);
                if (after != null)
                {
                    return after;
                }
            }
        }

        /// <summary>
        /// Returns an immutable snapshot of all registered prefixes.
        /// </summary>
        public ImmutableDictionary<string, IEdmTypeReference> Snapshot() => this.map;

        /// <summary>
        /// Try to get the Edm type reference associated with a literal prefix.
        /// </summary>
        public bool TryGet(string literalPrefix, out IEdmTypeReference edmType)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, nameof(literalPrefix));

            return this.map.TryGetValue(literalPrefix, out edmType);
        }

        /// <summary>
        /// Adds a literal prefix and associated Edm type if not already present.
        /// </summary>
        /// <returns><c>true</c> if added; <c>false</c> if it already existed.</returns>
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
        /// Remove a literal prefix by name.
        /// </summary>
        /// <returns><c>true</c> if removed; <c>false</c> if not found.</returns>>
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
        /// Generic CAS loop for immutable references.
        /// Returns the updated value on success; null indicates no update.
        /// </summary>
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
