//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// A segment representing a key lookup in a path.
    /// </summary>
    public sealed class KeySegment : ODataPathSegment
    {
        /// <summary>
        /// The set of key property names and the values to be used in searching for the given item.
        /// </summary>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Using key value pair is exactly what we want here.")]
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
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Using key value pair is exactly what we want here.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using key value pair is exactly what we want here.")]
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        public KeySegment(IEnumerable<KeyValuePair<string, object>> keys, IEdmEntityType edmType, IEdmNavigationSource navigationSource)
        {
            this.keys = new ReadOnlyCollection<KeyValuePair<string, object>>(keys.ToList());
            this.edmType = edmType;
            this.navigationSource = navigationSource;

            // Check that the type they gave us is related to the type of the set
            if (navigationSource != null)
            {
                UriParserErrorHelper.ThrowIfTypesUnrelated(edmType, navigationSource.EntityType(), "KeySegments");
            }
        }

        /// <summary>
        /// Gets the set of key property names and the values to be used in searching for the given item.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using key value pair is exactly what we want here.")]
        public IEnumerable<KeyValuePair<string, object>> Keys
        {
            [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Using key value pair is exactly what we want here.")]
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
