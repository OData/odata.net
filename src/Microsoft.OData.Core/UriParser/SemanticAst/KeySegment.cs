//---------------------------------------------------------------------
// <copyright file="KeySegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// A segment representing a key lookup in a path.
    /// </summary>
    public sealed class KeySegment : ODataPathSegment
    {
        /// <summary>
        /// The set of key property names and the values to be used in searching for the given item.
        /// </summary>
        private readonly ReadOnlyCollection<KeyValuePair<string, object>> keys;

        /// <summary>
        /// The type of the item this key returns.
        /// </summary>
        private readonly IEdmEntityType edmType;

        /// <summary>
        /// The navigation source that this key is used to search.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Construct a Segment that represents a key lookup.
        /// </summary>
        /// <param name="keys">The set of key property names and the values to be used in searching for the given item.</param>
        /// <param name="edmType">The type of the item this key returns.</param>
        /// <param name="navigationSource">The navigation source that this key is used to search.</param>
        /// <exception cref="ODataException">Throws if the input entity set is not related to the input type.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using key value pair is exactly what we want here.")]
        public KeySegment(IEnumerable<KeyValuePair<string, object>> keys, IEdmEntityType edmType, IEdmNavigationSource navigationSource)
        {
            this.keys = new ReadOnlyCollection<KeyValuePair<string, object>>(keys.ToList());
            this.edmType = edmType;
            this.navigationSource = navigationSource;
            this.SingleResult = true;

            // Check that the type they gave us is related to the type of the set
            if (navigationSource != null)
            {
                ExceptionUtil.ThrowIfTypesUnrelated(edmType, navigationSource.EntityType(), "KeySegments");
            }
        }

        /// <summary>
        /// Construct a Segment that represents a key lookup.
        /// </summary>
        /// <param name="previous">The segment to apply the key to.</param>
        /// <param name="keys">The set of key property names and the values to be used in searching for the given item.</param>
        /// <param name="edmType">The type of the item this key returns.</param>
        /// <param name="navigationSource">The navigation source that this key is used to search.</param>
        /// <exception cref="ODataException">Throws if the input entity set is not related to the input type.</exception>
        public KeySegment(ODataPathSegment previous, IEnumerable<KeyValuePair<string, object>> keys, IEdmEntityType edmType, IEdmNavigationSource navigationSource)
            : this(keys, edmType, navigationSource)
        {
            if (previous != null)
            {
                this.CopyValuesFrom(previous);
                this.SingleResult = true;
            }
        }

        /// <summary>
        /// Gets the set of key property names and the values to be used in searching for the given item.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using key value pair is exactly what we want here.")]
        public IEnumerable<KeyValuePair<string, object>> Keys
        {
            get { return this.keys.AsEnumerable(); }
        }

        /// <summary>
        /// Gets the type of the item this key returns
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.edmType; }
        }

        /// <summary>
        /// Gets the navigation source that this key is used to search.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Translate a <see cref="KeySegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="KeySegment"/> using an instance of <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "handler");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">the other segment to check.</param>
        /// <returns>true if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            KeySegment otherKeySegment = other as KeySegment;
            return otherKeySegment != null &&
                   otherKeySegment.Keys.SequenceEqual(this.Keys) &&
                   otherKeySegment.EdmType == this.edmType &&
                   otherKeySegment.NavigationSource == this.navigationSource;
        }
    }
}
