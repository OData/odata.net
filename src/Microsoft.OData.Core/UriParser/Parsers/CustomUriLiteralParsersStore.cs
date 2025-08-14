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
    using System.Runtime.Serialization;
    using System.Threading;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// Thread-safe store for custom URI literal parsers associated with an EDM model.
    /// Maintains both:
    ///  - global parsers (called for any literal),
    ///  - type-specific parsers (called when a target IEdmTypeReference is known).
    /// </summary>
    internal sealed class CustomUriLiteralParsersStore
    {
        private State state = State.Empty;

        private CustomUriLiteralParsersStore()
        {
        }

        public static CustomUriLiteralParsersStore GetOrCreate(IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, nameof(model));

            while (true)
            {
                CustomUriLiteralParsersStore existing = model.GetAnnotationValue<CustomUriLiteralParsersStore>(model);
                if (existing != null)
                {
                    return existing;
                }

                CustomUriLiteralParsersStore created = new CustomUriLiteralParsersStore();
                model.SetAnnotationValue(model, created);

                CustomUriLiteralParsersStore after = model.GetAnnotationValue<CustomUriLiteralParsersStore>(model);
                if (after != null)
                {
                    return after;
                }

            }
        }

        public ImmutableDictionary<IEdmTypeReference, IUriLiteralParser> SnapshotByType => this.state.CustomUriLiteralParsersByEdmType;

        public ImmutableHashSet<IUriLiteralParser> Snapshot => this.state.CustomUriLiteralParsers;

        public bool Contains(IUriLiteralParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(parser, nameof(parser));

            return this.state.CustomUriLiteralParsers.Contains(parser);
        }

        public bool TryGet(IEdmTypeReference edmTypeReference, out IUriLiteralParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(edmTypeReference, nameof(edmTypeReference));

            return state.CustomUriLiteralParsersByEdmType.TryGetValue(edmTypeReference, out parser);
        }

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
                        if (Equals(kvPair.Key, parser)) // Value equality?
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


        /// <summary>
        /// Single immutable state for atomic updates of both maps.
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
    }
}
