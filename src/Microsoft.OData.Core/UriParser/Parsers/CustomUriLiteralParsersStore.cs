//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralParsersAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Threading;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Thread-safe, per-<see cref="IEdmModel"/> store for custom URI literal parsers.
    /// Supports:
    ///  - Unbound parsers (invoked for any literal, regardless of type).
    ///  - Type-specific parsers (used when a target <see cref="IEdmTypeReference"/> is known).
    /// </summary>
    /// <remarks>
    /// Concurrency guarantees:
    /// 1. All operations are thread-safe.
    /// 2. Updates are atomic and readers always observe a consistent snapshot.
    /// 3. Mutations replace an immutable state object, ensuring no partial updates are visible.
    /// 4. Store instance creation per model is serialized using a per-model gate to avoid duplicate instances.
    /// </remarks>
    internal sealed class CustomUriLiteralParsersStore
    {
        // Entire logical state (both maps) is swapped atomically.
        private State state = State.Empty;

        // Per-model gate (lazy) for first-time store creation.
        private static readonly ConditionalWeakTable<IEdmModel, object> lockPerModel =
            new ConditionalWeakTable<IEdmModel, object>();

        private CustomUriLiteralParsersStore()
        {
        }

        /// <summary>
        /// Gets the store instance associated with the specified <paramref name="model"/>, creating and annotating it if it does not exist.
        /// A per-model lock ensures only one instance is created under concurrent access.
        /// </summary>
        /// <param name="model">The Edm model for which to obtain the store.</param>
        /// <returns>The existing or newly created <see cref="CustomUriLiteralParsersStore"/>.</returns>
        public static CustomUriLiteralParsersStore GetOrCreate(IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, nameof(model));

            CustomUriLiteralParsersStore existing = model.GetAnnotationValue<CustomUriLiteralParsersStore>(model);
            if (existing != null)
            {
                return existing;
            }

            object gate = lockPerModel.GetValue(model, _ => new object());
            lock (gate)
            {
                existing = model.GetAnnotationValue<CustomUriLiteralParsersStore>(model);
                if (existing != null)
                {
                    return existing;
                }

                CustomUriLiteralParsersStore created = new CustomUriLiteralParsersStore();
                model.SetAnnotationValue(model, created);

                return created;
            }
        }

        /// <summary>
        /// Returns a point-in-time snapshot of the registered type-specific parsers.
        /// </summary>
        /// <returns>
        /// A read-only dictionary mapping <see cref="IEdmTypeReference"/> to the associated parser
        /// at the time of the call.
        /// </returns>
        /// <remarks>
        /// Thread safety: Safe to call concurrently. The returned dictionary is immutable; subsequent updates
        /// to the store are not reflected unless <see cref="SnapshotByEdmType()"/> is called again.
        /// </remarks>
        public IReadOnlyDictionary<IEdmTypeReference, IUriLiteralParser> SnapshotByEdmType() => this.state.CustomUriLiteralParsersByEdmType;

        /// <summary>
        /// Returns a point-in-time snapshot of the registered unbound parsers.
        /// </summary>
        /// <returns>
        /// A read-only set of <see cref="IUriLiteralParser"/> instances representing the unbound parsers
        /// at the time of the call.
        /// </returns>
        /// <remarks>
        /// Thread safety: Safe to call concurrently. The returned set is immutable; subsequent updates
        /// to the store are not reflected unless <see cref="Snapshot()"/> is called again.
        /// </remarks>
        public IReadOnlySet<IUriLiteralParser> Snapshot() => this.state.CustomUriLiteralParsers;

        /// <summary>
        /// Determines whether the store contains the specified unbound parser.
        /// </summary>
        /// <param name="parser">The custom URI literal parser to check.</param>
        /// <returns><c>true</c> if the parser is registered; otherwise <c>false</c>.</returns>
        public bool Contains(IUriLiteralParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(parser, nameof(parser));

            return this.state.CustomUriLiteralParsers.Contains(parser);
        }

        /// <summary>
        /// Attempts to retrieve a type-specific parser for the given Edm type.
        /// </summary>
        /// <param name="edmTypeReference">The Edm type.</param>
        /// <param name="parser">When this method returns, contains the parser associated with the Edm type, if found.</param>
        /// <returns><c>true</c> if a parser for the specified Edm type was found; otherwise <c>false</c>.</returns>
        public bool TryGet(IEdmTypeReference edmTypeReference, out IUriLiteralParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(edmTypeReference, nameof(edmTypeReference));

            return state.CustomUriLiteralParsersByEdmType.TryGetValue(edmTypeReference, out parser);
        }

        /// <summary>
        /// Registers an unbound parser if it is not already registered.
        /// </summary>
        /// <param name="parser">The custom URI literal parser to register.</param>
        /// <returns><c>true</c> if the parser was successfully registered; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Thread safety: All operations are atomic and readers always observe a consistent snapshot.
        /// Updates are applied by replacing an immutable state object, ensuring no partial updates are visible.
        /// </remarks>
        public bool Add(IUriLiteralParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(parser, nameof(parser));

            return CompareExchangeLoop(
                ref this.state,
                snapshot =>
                {
                    if (snapshot.CustomUriLiteralParsers.Contains(parser))
                    {
                        return null; // No change
                    }

                    ImmutableHashSet<IUriLiteralParser> customUriLiteralParsers = snapshot.CustomUriLiteralParsers.Add(parser);

                    return new State(snapshot.CustomUriLiteralParsersByEdmType, customUriLiteralParsers);
                }) != null;
        }

        /// <summary>
        /// Registers a parser for a specific Edm type if one is not already registered for that type.
        /// </summary>
        /// <param name="edmTypeReference">The Edm type.</param>
        /// <param name="parser">The custom URI literal parser to register.</param>
        /// <returns><c>true</c> if the parser was successfully registered; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Thread safety: All operations are atomic and readers always observe a consistent snapshot.
        /// Updates are applied by replacing an immutable state object, ensuring no partial updates are visible.
        /// </remarks>
        public bool Add(IEdmTypeReference edmTypeReference, IUriLiteralParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(edmTypeReference, nameof(edmTypeReference));
            ExceptionUtils.CheckArgumentNotNull(parser, nameof(parser));

            return CompareExchangeLoop(
                ref this.state,
                snapshot =>
                {
                    if (snapshot.CustomUriLiteralParsersByEdmType.ContainsKey(edmTypeReference))
                    {
                        return null; // No change
                    }

                    ImmutableDictionary<IEdmTypeReference, IUriLiteralParser> customUriLiteralParsersByEdmType = snapshot.CustomUriLiteralParsersByEdmType.Add(edmTypeReference, parser);

                    return new State(customUriLiteralParsersByEdmType, snapshot.CustomUriLiteralParsers);
                }) != null;
        }

        /// <summary>
        /// Removes the specified unbound parser and any associated type-specific parsers.
        /// </summary>
        /// <param name="parser">The custom URI literal parser to remove.</param>
        /// <returns><c>true</c> if the parser existed and was removed; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Thread safety: All operations are atomic and readers always observe a consistent snapshot.
        /// Updates are applied by replacing an immutable state object, ensuring no partial updates are visible.
        /// </remarks>
        public bool Remove(IUriLiteralParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(parser, nameof(parser));

            State mutatedState = CompareExchangeLoop(
                ref this.state,
                snapshot =>
                {
                    ImmutableHashSet<IUriLiteralParser> mutatedCustomUriLiteralParsers = snapshot.CustomUriLiteralParsers.Remove(parser);
                    bool customUriLiteralParsersMutated = !ReferenceEquals(mutatedCustomUriLiteralParsers, snapshot.CustomUriLiteralParsers);

                    ImmutableDictionary<IEdmTypeReference, IUriLiteralParser>.Builder builder = snapshot.CustomUriLiteralParsersByEdmType.ToBuilder();
                    int numRemoved = 0;
                    foreach (KeyValuePair<IEdmTypeReference, IUriLiteralParser> kvPair in snapshot.CustomUriLiteralParsersByEdmType)
                    {
                        if (Equals(kvPair.Value, parser)) // Value equality?
                        {
                            builder.Remove(kvPair.Key);
                            numRemoved++;
                        }
                    }

                    ImmutableDictionary<IEdmTypeReference, IUriLiteralParser> mutatedCustomUriLiteralParsersByEdmType = numRemoved > 0 ? builder.ToImmutable() : snapshot.CustomUriLiteralParsersByEdmType;
                    bool customUriLiteralParsersByEdmTypeMutated = !ReferenceEquals(mutatedCustomUriLiteralParsersByEdmType, snapshot.CustomUriLiteralParsersByEdmType);
                    
                    if (!customUriLiteralParsersMutated && !customUriLiteralParsersByEdmTypeMutated)
                    {
                        return null; // No change
                    }

                    return new State(mutatedCustomUriLiteralParsersByEdmType, mutatedCustomUriLiteralParsers);
                });

            // true only if we applied new state
            return mutatedState != null;
        }

        /// <summary>
        /// Removes the parser associated with the specified Edm type.
        /// </summary>
        /// <param name="edmTypeReference">The Edm type whose parser should be removed.</param>
        /// <returns><c>true</c> if the parser existed and was removed; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Thread safety: All operations are atomic and readers always observe a consistent snapshot.
        /// Updates are applied by replacing an immutable state object, ensuring no partial updates are visible.
        /// </remarks>
        public bool Remove(IEdmTypeReference edmTypeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(edmTypeReference, nameof(edmTypeReference));

            return CompareExchangeLoop(
                ref this.state,
                snapshot =>
                {
                    ImmutableDictionary<IEdmTypeReference, IUriLiteralParser> mutatedCustomUriLiteralParsersByEdmType = snapshot.CustomUriLiteralParsersByEdmType.Remove(edmTypeReference);

                    // If key not present, ImmutableDictionary.Remove returns the same instance
                    if (ReferenceEquals(mutatedCustomUriLiteralParsersByEdmType, snapshot.CustomUriLiteralParsersByEdmType))
                    {
                        return null; // No change
                    }

                    return new State(mutatedCustomUriLiteralParsersByEdmType, snapshot.CustomUriLiteralParsers);
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

        /// <summary>
        /// Structural equality comparer for Edm type references, consistent with EdmElementComparer.IsEquivalentTo.
        /// </summary>
        /// <remarks>
        /// Used as a key comparer for dictionaries that map custom URI literal parsers to EDM types.
        /// </remarks>
        private sealed class EdmTypeEqualityComparer : IEqualityComparer<IEdmTypeReference>
        {
            public static readonly EdmTypeEqualityComparer Instance = new EdmTypeEqualityComparer();

            public bool Equals(IEdmTypeReference obj, IEdmTypeReference other)
            {
                return EdmElementComparer.IsEquivalentTo(obj, other);
            }

            public int GetHashCode(IEdmTypeReference obj)
            {

                if (obj == null) return 0;

                // Conservative hash consistent with Equals: full name + nullability (+ simple handling for collections).
                // Collisions are acceptable; correctness relies on Equals.
                unchecked
                {
                    int hash = obj.FullName()?.GetHashCode(StringComparison.Ordinal) ?? 0;
                    hash = (hash * 31) + (obj.IsNullable ? 1 : 0);

                    if (obj.IsCollection())
                    {
                        var element = ((IEdmCollectionTypeReference)obj).ElementType();
                        hash = (hash * 31) + (element.FullName()?.GetHashCode(StringComparison.Ordinal) ?? 0);
                        hash = (hash * 31) + (element.IsNullable ? 1 : 0);
                    }

                    return hash;
                }
            }
        }

        #region Internal State

        /// <summary>
        /// Single immutable state for atomic publication of both collections.
        /// </summary
        private sealed class State
        {
            public readonly ImmutableDictionary<IEdmTypeReference, IUriLiteralParser> CustomUriLiteralParsersByEdmType;
            public readonly ImmutableHashSet<IUriLiteralParser> CustomUriLiteralParsers;

            public static readonly State Empty = new State(
                ImmutableDictionary.Create<IEdmTypeReference, IUriLiteralParser>(EdmTypeEqualityComparer.Instance),
                ImmutableHashSet.Create<IUriLiteralParser>(ReferenceEqualityComparer<IUriLiteralParser>.Instance));

            public State(
                ImmutableDictionary<IEdmTypeReference, IUriLiteralParser> customUriLiteralParsersByEdmType,
                ImmutableHashSet<IUriLiteralParser> customUriLiteralParsers)
            {
                CustomUriLiteralParsersByEdmType = customUriLiteralParsersByEdmType;
                CustomUriLiteralParsers = customUriLiteralParsers;
            }
        }
        
        #endregion Internal State
    }
}
