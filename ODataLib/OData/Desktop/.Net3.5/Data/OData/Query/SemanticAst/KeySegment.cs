//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
