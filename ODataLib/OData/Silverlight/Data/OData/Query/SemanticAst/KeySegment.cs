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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

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
        /// The entity set that this key is used to search.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// Construct a Segment that represents a key lookup.
        /// </summary>
        /// <param name="keys">The set of key property names and the values to be used in searching for the given item.</param>
        /// <param name="edmType">The type of the item this key returns.</param>
        /// <param name="entitySet">The entity set that this key is used to search.</param>
        /// <exception cref="ODataException">Throws if the input entity set is not related to the input type.</exception>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Using key value pair is exactly what we want here.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using key value pair is exactly what we want here.")]
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        public KeySegment(IEnumerable<KeyValuePair<string, object>> keys, IEdmEntityType edmType, IEdmEntitySet entitySet)
        {
            DebugUtils.CheckNoExternalCallers();
            this.keys = new ReadOnlyCollection<KeyValuePair<string, object>>(keys.ToList());
            this.edmType = edmType;
            this.entitySet = entitySet;

            // Check that the type they gave us is related to the type of the set
            if (entitySet != null)
            {
                UriParserErrorHelper.ThrowIfTypesUnrelated(edmType, entitySet.ElementType, "KeySegments");
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
        /// Gets the entity set that this key is used to search.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Translate a <see cref="KeySegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T Translate<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="KeySegment"/> using an instance of <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void Handle(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "translator");
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            KeySegment otherKeySegment = other as KeySegment;
            return otherKeySegment != null &&
                   otherKeySegment.Keys.SequenceEqual(this.Keys) &&
                   otherKeySegment.EdmType == this.edmType &&
                   otherKeySegment.EntitySet == this.entitySet;
        }
    }
}
