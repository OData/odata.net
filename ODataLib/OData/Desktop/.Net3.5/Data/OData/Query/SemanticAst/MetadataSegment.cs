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

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// A segment representing $metadata in a path.
    /// </summary>
    public sealed class MetadataSegment : ODataPathSegment
    {
        /// <summary>
        /// Gets the singleton instance of MetadataSegment
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "MetadataSegment is immutable")]
        public static readonly MetadataSegment Instance = new MetadataSegment();

        /// <summary>
        /// Build a segment to represent $metadata
        /// </summary>
        private MetadataSegment()
        {
            this.Identifier = UriQueryConstants.MetadataSegment;
            this.TargetKind = RequestTargetKind.Metadata;
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="MetadataSegment"/>, which is always null.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return null; }
        }

        /// <summary>
        /// Translate a <see cref="PathSegmentTranslator{T}"/>.
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
        /// Translate a <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the translator interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void Handle(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "translator");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another.
        /// </summary>
        /// <param name="other">the other segment to check.</param>
        /// <returns>true if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            return other is MetadataSegment;
        }
    }
}
