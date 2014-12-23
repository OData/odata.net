//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;

    #endregion Namespaces

    /// <summary>
    /// A segment representing a cast on the previous segment to another type.
    /// </summary>
    public sealed class TypeSegment : ODataPathSegment
    {
        /// <summary>
        /// The target type of this type segment.
        /// </summary>
        private readonly IEdmType edmType;

        /// <summary>
        /// The navigation source containing the entities that we are casting.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Build a type segment using the given <paramref name="edmType"/>.
        /// </summary>
        /// <param name="edmType">The target type of this segment, which may be collection type.</param>
        /// <param name="navigationSource">The navigation source containing the entities that we are casting. This can be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input edmType is null.</exception>
        /// <exception cref="ODataException">Throws if the input edmType is not relaed to the type of elements in the input entitySet.</exception>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        public TypeSegment(IEdmType edmType, IEdmNavigationSource navigationSource) 
        {
            ExceptionUtils.CheckArgumentNotNull(edmType, "type");

            this.edmType = edmType;
            this.navigationSource = navigationSource;

            this.TargetEdmType = edmType;
            this.TargetEdmNavigationSource = navigationSource;

            // Check that the type they gave us is related to the type of the set
            if (navigationSource != null)
            {
                ExceptionUtil.ThrowIfTypesUnrelated(edmType, navigationSource.EntityType(), "TypeSegments");
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="TypeSegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.edmType; }
        }

        /// <summary>
        /// Gets the navigation source containing the entities that we are casting.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Translate a <see cref="TypeSegment"/> into another type using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle a <see cref="TypeSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
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
            TypeSegment otherType = other as TypeSegment;
            return otherType != null && otherType.EdmType == this.EdmType;
        }
    }
}
